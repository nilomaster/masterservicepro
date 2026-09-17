<?php
// API Endpoint to generate Mercado Pago Pix QRCode and Copia e Cola
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

// Retrieve input params
$chave = '';
if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $rawInput = file_get_contents('php://input');
    $jsonData = json_decode($rawInput, true);
    $chave = trim($jsonData['chave'] ?? ($_POST['chave'] ?? ''));
} else {
    $chave = trim($_GET['chave'] ?? '');
}

if (empty($chave)) {
    sendJsonResponse([
        'success' => false,
        'message' => 'Chave de licença não informada.'
    ], 400);
}

$pdo = getDbConnection();

// Look up license
$stmt = $pdo->prepare('SELECT * FROM licencas WHERE chave_licenca = :chave LIMIT 1');
$stmt->execute(['chave' => $chave]);
$licenca = $stmt->fetch();

if (!$licenca) {
    sendJsonResponse([
        'success' => false,
        'message' => 'Licença não encontrada.'
    ], 404);
}

$valor = (float)$licenca['valor_mensalidade'];
if ($valor <= 0) {
    $valor = DEFAULT_MONTHLY_PRICE;
}

// Check for recent pending payment (created within last 15 minutes) to avoid duplicates
$stmtRecent = $pdo->prepare("
    SELECT * FROM pagamentos_pix 
    WHERE id_licenca = :id_licenca AND status = 'pending' 
      AND data_criacao >= DATE_SUB(NOW(), INTERVAL 15 MINUTE)
    ORDER BY id DESC LIMIT 1
");
$stmtRecent->execute(['id_licenca' => $licenca['id']]);
$recentPix = $stmtRecent->fetch();

if ($recentPix && !empty($recentPix['qr_code_base64'])) {
    sendJsonResponse([
        'success' => true,
        'payment_id' => (string)$recentPix['mp_payment_id'],
        'chave' => $licenca['chave_licenca'],
        'valor' => (float)$recentPix['valor'],
        'qr_code_base64' => $recentPix['qr_code_base64'],
        'copia_cola' => $recentPix['copia_cola'],
        'status' => 'pending',
        'is_reused' => true,
        'message' => 'Cobrança Pix ativa existente recuperada com sucesso.'
    ]);
}

// Prepare Mercado Pago API request
$payerEmail = 'financeiro@' . preg_replace('/[^a-zA-Z0-9]/', '', strtolower($licenca['chave_licenca'])) . '.com';
$mpToken = trim(getMercadoPagoToken());

$isDemoMode = ($mpToken === 'APP_USR-SEU-ACCESS-TOKEN-AQUI' || empty($mpToken));

if ($isDemoMode) {
    // Demo / Simulation mode if token is not configured yet
    $simulatedPaymentId = time() . rand(1000, 9999);
    $simulatedCopiaCola = '00020126580014br.gov.bcb.pix0136' . md5($licenca['chave_licenca']) . '520400005303986540' . number_format($valor, 2, '', '') . '5802BR5915MasterServicePro6009SaoPaulo62070503***6304DEMO';
    
    // Generate a simple 1x1 transparent/sample base64 for fallback or dummy SVG
    $simulatedQrBase64 = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==';

    $insertStmt = $pdo->prepare("
        INSERT INTO pagamentos_pix 
        (id_licenca, mp_payment_id, chave_licenca, qr_code, qr_code_base64, copia_cola, valor, status, data_criacao)
        VALUES 
        (:id_licenca, :mp_id, :chave, :qr_code, :qr_base64, :copia_cola, :valor, 'pending', NOW())
    ");
    $insertStmt->execute([
        'id_licenca' => $licenca['id'],
        'mp_id' => $simulatedPaymentId,
        'chave' => $licenca['chave_licenca'],
        'qr_code' => $simulatedCopiaCola,
        'qr_base64' => $simulatedQrBase64,
        'copia_cola' => $simulatedCopiaCola,
        'valor' => $valor
    ]);

    sendJsonResponse([
        'success' => true,
        'payment_id' => (string)$simulatedPaymentId,
        'chave' => $licenca['chave_licenca'],
        'valor' => $valor,
        'qr_code_base64' => $simulatedQrBase64,
        'copia_cola' => $simulatedCopiaCola,
        'status' => 'pending',
        'is_demo' => true,
        'message' => 'Cobrança Pix simulada gerada (configure MP_ACCESS_TOKEN no config.php para produção).'
    ]);
}

// Call Mercado Pago API v1/payments
$endpoint = 'https://api.mercadopago.com/v1/payments';
$payload = [
    'transaction_amount' => (float)$valor,
    'description' => 'Renovacao Licenca ' . ($licenca['sistema'] ?? 'MasterServicePro') . ' - ' . $licenca['chave_licenca'],
    'payment_method_id' => 'pix',
    'payer' => [
        'email' => $payerEmail,
        'first_name' => substr($licenca['cliente_nome'], 0, 30)
    ]
];

$ch = curl_init($endpoint);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_POST, true);
curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($payload));
curl_setopt($ch, CURLOPT_HTTPHEADER, [
    'Content-Type: application/json',
    'Authorization: Bearer ' . $mpToken,
    'X-Idempotency-Key: ' . uniqid('msp_', true)
]);
curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, true);

$response = curl_exec($ch);
$httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
$curlError = curl_error($ch);
curl_close($ch);

if ($curlError) {
    sendJsonResponse([
        'success' => false,
        'message' => 'Falha de comunicacao com Mercado Pago: ' . $curlError
    ], 500);
}

$mpData = json_decode($response, true);

if ($httpCode !== 200 && $httpCode !== 201) {
    sendJsonResponse([
        'success' => false,
        'http_code' => $httpCode,
        'mp_error' => $mpData,
        'message' => 'Erro retornado pela API do Mercado Pago. Verifique suas credenciais no config.php.'
    ], 500);
}

$paymentId = $mpData['id'] ?? null;
$transactionData = $mpData['point_of_interaction']['transaction_data'] ?? null;

if (!$paymentId || !$transactionData) {
    sendJsonResponse([
        'success' => false,
        'message' => 'Resposta incompleta do Mercado Pago ao gerar Pix.'
    ], 500);
}

$qrCode = $transactionData['qr_code'] ?? '';
$qrCodeBase64 = $transactionData['qr_code_base64'] ?? '';

// Save in database
$insertStmt = $pdo->prepare("
    INSERT INTO pagamentos_pix 
    (id_licenca, mp_payment_id, chave_licenca, qr_code, qr_code_base64, copia_cola, valor, status, data_criacao)
    VALUES 
    (:id_licenca, :mp_id, :chave, :qr_code, :qr_base64, :copia_cola, :valor, 'pending', NOW())
");
$insertStmt->execute([
    'id_licenca' => $licenca['id'],
    'mp_id' => $paymentId,
    'chave' => $licenca['chave_licenca'],
    'qr_code' => $qrCode,
    'qr_base64' => $qrCodeBase64,
    'copia_cola' => $qrCode,
    'valor' => $valor
]);

sendJsonResponse([
    'success' => true,
    'payment_id' => (string)$paymentId,
    'chave' => $licenca['chave_licenca'],
    'valor' => $valor,
    'qr_code_base64' => $qrCodeBase64,
    'copia_cola' => $qrCode,
    'status' => 'pending',
    'message' => 'Pix QRCode gerado com sucesso.'
]);
