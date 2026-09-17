<?php
// API Endpoint to check and validate software license
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

// Retrieve input params (accepts GET or POST JSON / form-data)
$chave = '';
$hwid = '';
$sistema = '';

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $rawInput = file_get_contents('php://input');
    $jsonData = json_decode($rawInput, true);
    if (is_array($jsonData)) {
        $chave = trim($jsonData['chave'] ?? '');
        $hwid = trim($jsonData['hwid'] ?? '');
        $sistema = trim($jsonData['sistema'] ?? '');
    } else {
        $chave = trim($_POST['chave'] ?? '');
        $hwid = trim($_POST['hwid'] ?? '');
        $sistema = trim($_POST['sistema'] ?? '');
    }
} else {
    $chave = trim($_GET['chave'] ?? '');
    $hwid = trim($_GET['hwid'] ?? '');
    $sistema = trim($_GET['sistema'] ?? '');
}

if (empty($chave)) {
    sendJsonResponse([
        'success' => false,
        'status' => 'missing_key',
        'message' => 'Chave de licença não fornecida.'
    ], 400);
}

$pdo = getDbConnection();

// Look up license in database
$stmt = $pdo->prepare('SELECT * FROM licencas WHERE chave_licenca = :chave LIMIT 1');
$stmt->execute(['chave' => $chave]);
$licenca = $stmt->fetch();

if (!$licenca) {
    sendJsonResponse([
        'success' => false,
        'status' => 'not_found',
        'message' => 'Chave de licença inválida ou inexistente.'
    ], 404);
}

// Check if blocked by admin
if ($licenca['status'] === 'bloqueada') {
    sendJsonResponse([
        'success' => false,
        'status' => 'blocked',
        'message' => 'Esta licença foi suspensa pelo suporte. Entre em contato.'
    ], 403);
}

// Bind or verify Hardware ID (HWID)
if (!empty($hwid)) {
    if (empty($licenca['hwid'])) {
        // First machine activation - lock serial to this HWID
        $updateHwid = $pdo->prepare('UPDATE licencas SET hwid = :hwid WHERE id = :id');
        $updateHwid->execute(['hwid' => $hwid, 'id' => $licenca['id']]);
        $licenca['hwid'] = $hwid;
    } else if ($licenca['hwid'] !== $hwid) {
        sendJsonResponse([
            'success' => false,
            'status' => 'hwid_mismatch',
            'message' => 'Esta chave de licença já está vinculada a outro computador.'
        ], 403);
    }
}

// Check expiration date
$now = new DateTime();
$vencimento = new DateTime($licenca['data_vencimento']);

if ($now > $vencimento) {
    // License has expired
    if ($licenca['status'] !== 'vencida') {
        $updateStatus = $pdo->prepare("UPDATE licencas SET status = 'vencida' WHERE id = :id");
        $updateStatus->execute(['id' => $licenca['id']]);
    }

    sendJsonResponse([
        'success' => true,
        'status' => 'expired',
        'sistema' => $licenca['sistema'] ?? 'MasterServicePro',
        'cliente' => $licenca['cliente_nome'],
        'chave' => $licenca['chave_licenca'],
        'vencimento' => $vencimento->format('Y-m-d H:i:s'),
        'vencimento_br' => $vencimento->format('d/m/Y'),
        'dias_restantes' => 0,
        'valor_mensalidade' => (float)$licenca['valor_mensalidade'],
        'message' => 'Sua licença expirou. Efetue o pagamento para renovar o acesso.'
    ]);
}

// License is active
$interval = $now->diff($vencimento);
$diasRestantes = (int)$interval->format('%r%a');

$signature = generateLicenseSignature(
    $licenca['chave_licenca'],
    $licenca['hwid'] ?? '',
    $vencimento->format('Y-m-d H:i:s'),
    'ativa'
);

sendJsonResponse([
    'success' => true,
    'status' => 'active',
    'sistema' => $licenca['sistema'] ?? 'MasterServicePro',
    'cliente' => $licenca['cliente_nome'],
    'chave' => $licenca['chave_licenca'],
    'vencimento' => $vencimento->format('Y-m-d H:i:s'),
    'vencimento_br' => $vencimento->format('d/m/Y'),
    'dias_restantes' => $diasRestantes,
    'valor_mensalidade' => (float)$licenca['valor_mensalidade'],
    'signature' => $signature,
    'message' => 'Licença ativa e autorizada.'
]);
