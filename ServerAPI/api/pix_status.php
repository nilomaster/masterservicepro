<?php
// API Endpoint to check Pix payment status and auto-renew license
// All comments must use ASCII characters only.

require_once __DIR__ . '/../config.php';

// Handle preflight requests
if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    header('Access-Control-Allow-Origin: *');
    header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
    header('Access-Control-Allow-Headers: Content-Type, Authorization');
    http_response_code(200);
    exit;
}

$paymentId = trim($_GET['payment_id'] ?? ($_POST['payment_id'] ?? ''));
$chave = trim($_GET['chave'] ?? ($_POST['chave'] ?? ''));
$simulateApprove = isset($_GET['simulate_approve']) || isset($_POST['simulate_approve']);

if (empty($paymentId) && empty($chave)) {
    sendJsonResponse([
        'success' => false,
        'message' => 'Informe o payment_id ou a chave da licença.'
    ], 400);
}

$pdo = getDbConnection();

// Fetch payment record
if (!empty($paymentId)) {
    $stmt = $pdo->prepare('SELECT p.*, l.data_vencimento, l.status as licenca_status FROM pagamentos_pix p JOIN licencas l ON p.id_licenca = l.id WHERE p.mp_payment_id = :mp_id LIMIT 1');
    $stmt->execute(['mp_id' => $paymentId]);
    $pixRecord = $stmt->fetch();
} else {
    $stmt = $pdo->prepare('SELECT p.*, l.data_vencimento, l.status as licenca_status FROM pagamentos_pix p JOIN licencas l ON p.id_licenca = l.id WHERE p.chave_licenca = :chave ORDER BY p.id DESC LIMIT 1');
    $stmt->execute(['chave' => $chave]);
    $pixRecord = $stmt->fetch();
}

if (!$pixRecord) {
    sendJsonResponse([
        'success' => false,
        'message' => 'Cobrança Pix não localizada no banco de dados.'
    ], 404);
}

// Function to process license renewal (+30 days)
function executeLicenseRenewal($pdo, $pixRecord) {
    $licencaId = $pixRecord['id_licenca'];
    $paymentId = $pixRecord['mp_payment_id'];
    
    // Calculate new expiration date
    $currentExpiration = new DateTime($pixRecord['data_vencimento']);
    $now = new DateTime();
    
    if ($now > $currentExpiration) {
        // Was expired: add 30 days from today
        $newExpiration = (clone $now)->modify('+30 days');
    } else {
        // Was still active (early renewal): add 30 days onto current expiration
        $newExpiration = (clone $currentExpiration)->modify('+30 days');
    }
    
    $newDateStr = $newExpiration->format('Y-m-d H:i:s');
    
    // Update license
    $upLic = $pdo->prepare("
        UPDATE licencas 
        SET status = 'ativa', 
            data_vencimento = :nova_data, 
            data_ultimo_pagamento = NOW() 
        WHERE id = :id
    ");
    $upLic->execute(['nova_data' => $newDateStr, 'id' => $licencaId]);
    
    // Update payment record
    $upPix = $pdo->prepare("
        UPDATE pagamentos_pix 
        SET status = 'approved', 
            data_aprovacao = NOW() 
        WHERE mp_payment_id = :mp_id
    ");
    $upPix->execute(['mp_id' => $paymentId]);
    
    return $newDateStr;
}

// Check if already approved
if ($pixRecord['status'] === 'approved') {
    sendJsonResponse([
        'success' => true,
        'status' => 'approved',
        'payment_id' => (string)$pixRecord['mp_payment_id'],
        'vencimento' => $pixRecord['data_vencimento'],
        'vencimento_br' => date('d/m/Y', strtotime($pixRecord['data_vencimento'])),
        'message' => 'Pagamento aprovado! Licença renovada com sucesso.'
    ]);
}

// Test / Simulator feature
if ($simulateApprove) {
    $newDate = executeLicenseRenewal($pdo, $pixRecord);
    sendJsonResponse([
        'success' => true,
        'status' => 'approved',
        'simulated' => true,
        'payment_id' => (string)$pixRecord['mp_payment_id'],
        'vencimento' => $newDate,
        'vencimento_br' => date('d/m/Y', strtotime($newDate)),
        'message' => 'Pagamento simulado com sucesso! Licença renovada por mais 30 dias.'
    ]);
}

// Consult Mercado Pago API if real credentials configured
$mpToken = trim(MP_ACCESS_TOKEN);
if (!empty($mpToken) && $mpToken !== 'APP_USR-SEU-ACCESS-TOKEN-AQUI') {
    $ch = curl_init('https://api.mercadopago.com/v1/payments/' . $pixRecord['mp_payment_id']);
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
        $mpStatus = $mpPayment['status'] ?? 'pending';
        
        if ($mpStatus === 'approved') {
            $newDate = executeLicenseRenewal($pdo, $pixRecord);
            sendJsonResponse([
                'success' => true,
                'status' => 'approved',
                'payment_id' => (string)$pixRecord['mp_payment_id'],
                'vencimento' => $newDate,
                'vencimento_br' => date('d/m/Y', strtotime($newDate)),
                'message' => 'Pagamento confirmado pelo Mercado Pago! Licença liberada com sucesso.'
            ]);
        } else if ($mpStatus === 'cancelled' || $mpStatus === 'rejected') {
            $upPix = $pdo->prepare("UPDATE pagamentos_pix SET status = :st WHERE mp_payment_id = :mp_id");
            $upPix->execute(['st' => $mpStatus, 'mp_id' => $pixRecord['mp_payment_id']]);
            
            sendJsonResponse([
                'success' => true,
                'status' => $mpStatus,
                'payment_id' => (string)$pixRecord['mp_payment_id'],
                'message' => 'Pagamento cancelado ou rejeitado no gateway.'
            ]);
        }
    }
}

// Still pending
sendJsonResponse([
    'success' => true,
    'status' => 'pending',
    'payment_id' => (string)$pixRecord['mp_payment_id'],
    'message' => 'Aguardando confirmação do pagamento Pix...'
]);
