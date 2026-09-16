<?php
// Mercado Pago Webhook / IPN Notification Endpoint
// All comments must use ASCII characters only.

require_once __DIR__ . '/../config.php';

$rawBody = file_get_contents('php://input');
$event = json_decode($rawBody, true);

$paymentId = null;

// Mercado Pago sends payment notifications either via query param or json body
if (isset($_GET['data_id'])) {
    $paymentId = $_GET['data_id'];
} else if (isset($_GET['id'])) {
    $paymentId = $_GET['id'];
} else if (isset($event['data']['id'])) {
    $paymentId = $event['data']['id'];
}

if (!$paymentId) {
    http_response_code(200);
    echo json_encode(['status' => 'ignored', 'message' => 'No payment id found']);
    exit;
}

$mpToken = trim(MP_ACCESS_TOKEN);
if (empty($mpToken) || $mpToken === 'APP_USR-SEU-ACCESS-TOKEN-AQUI') {
    http_response_code(200);
    echo json_encode(['status' => 'ignored', 'message' => 'Token not configured']);
    exit;
}

// Query payment details from Mercado Pago
$ch = curl_init('https://api.mercadopago.com/v1/payments/' . $paymentId);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_HTTPHEADER, [
    'Authorization: Bearer ' . $mpToken
]);
curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, true);

$response = curl_exec($ch);
$httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
curl_close($ch);

if ($httpCode === 200) {
    $mpPayment = json_decode($response, true);
    $status = $mpPayment['status'] ?? '';
    
    if ($status === 'approved') {
        $pdo = getDbConnection();
        $stmt = $pdo->prepare('SELECT p.*, l.data_vencimento FROM pagamentos_pix p JOIN licencas l ON p.id_licenca = l.id WHERE p.mp_payment_id = :mp_id LIMIT 1');
        $stmt->execute(['mp_id' => $paymentId]);
        $pixRecord = $stmt->fetch();
        
        if ($pixRecord && $pixRecord['status'] !== 'approved') {
            $currentExpiration = new DateTime($pixRecord['data_vencimento']);
            $now = new DateTime();
            
            if ($now > $currentExpiration) {
                $newExpiration = (clone $now)->modify('+30 days');
            } else {
                $newExpiration = (clone $currentExpiration)->modify('+30 days');
            }
            
            $newDateStr = $newExpiration->format('Y-m-d H:i:s');
            
            $upLic = $pdo->prepare("UPDATE licencas SET status = 'ativa', data_vencimento = :nova_data, data_ultimo_pagamento = NOW() WHERE id = :id");
            $upLic->execute(['nova_data' => $newDateStr, 'id' => $pixRecord['id_licenca']]);
            
            $upPix = $pdo->prepare("UPDATE pagamentos_pix SET status = 'approved', data_aprovacao = NOW() WHERE mp_payment_id = :mp_id");
            $upPix->execute(['mp_id' => $paymentId]);
        }
    }
}

http_response_code(200);
echo json_encode(['status' => 'processed']);
