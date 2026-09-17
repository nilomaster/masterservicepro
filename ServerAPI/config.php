<?php
// Configuration file for MasterDevSolutions Licensing and Pix Server
// All comments must use ASCII characters only.

// Database configuration
define('DB_HOST', 'mdev_solutions.mysql.dbaas.com.br');
define('DB_NAME', 'mdev_solutions');
define('DB_USER', 'mdev_solutions');
define('DB_PASS', 'Senhadb2026!@#');

// Default Fallback Credentials (can be updated dynamically via admin panel)
define('MP_ACCESS_TOKEN', 'APP_USR-SEU-ACCESS-TOKEN-AQUI');
define('DEFAULT_MONTHLY_PRICE', 80.00);
define('API_SECRET_SALT', 'MasterDevSolutions_Secret_Key_2026_Salt');
define('ADMIN_PASSWORD', 'admin123');

// Load Global Error Handler
require_once __DIR__ . '/error_handler.php';

// Helper function to establish database connection with PDO
function getDbConnection() {
    static $pdo = null;
    if ($pdo === null) {
        $dsn = 'mysql:host=' . DB_HOST . ';dbname=' . DB_NAME . ';charset=utf8mb4';
        $options = [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
            PDO::ATTR_EMULATE_PREPARES => false
        ];
        try {
            $pdo = new PDO($dsn, DB_USER, DB_PASS, $options);
            ensureSchemaUpgrades($pdo);
        } catch (PDOException $e) {
            showGlobalError(
                'db_connection',
                'Falha na Conexão com o Banco de Dados',
                'Não foi possível estabelecer conexão com o servidor MySQL configurado.',
                $e->getMessage(),
                500
            );
        }
    }
    return $pdo;
}

// Ensures required tables and columns exist automatically on older installs
function ensureSchemaUpgrades($pdo) {
    static $checked = false;
    if ($checked) return;
    $checked = true;

    try {
        // 1. Create configuracoes table if missing
        $pdo->exec("
            CREATE TABLE IF NOT EXISTS `configuracoes` (
              `chave` VARCHAR(50) NOT NULL PRIMARY KEY,
              `valor` TEXT NULL,
              `data_atualizacao` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
        ");

        // 2. Check if sistema column exists in licencas table
        $columns = $pdo->query("SHOW COLUMNS FROM `licencas` LIKE 'sistema'")->fetchAll();
        if (empty($columns)) {
            $pdo->exec("ALTER TABLE `licencas` ADD COLUMN `sistema` VARCHAR(100) NOT NULL DEFAULT 'MasterServicePro' AFTER `id`");
        }
    } catch (Exception $e) {
        // Silently continue if tables do not exist yet before initial import
    }
}

// Retrieves dynamic configuration from database with fallback
function getSystemConfig($key, $default = '') {
    try {
        $pdo = getDbConnection();
        $stmt = $pdo->prepare('SELECT valor FROM configuracoes WHERE chave = :chave LIMIT 1');
        $stmt->execute(['chave' => $key]);
        $row = $stmt->fetch();
        if ($row && $row['valor'] !== null && $row['valor'] !== '') {
            return $row['valor'];
        }
    } catch (Exception $e) { }

    return $default;
}

// Saves dynamic configuration in database
function setSystemConfig($key, $value) {
    try {
        $pdo = getDbConnection();
        $stmt = $pdo->prepare("
            INSERT INTO configuracoes (chave, valor) 
            VALUES (:chave, :valor) 
            ON DUPLICATE KEY UPDATE valor = :valor_up, data_atualizacao = NOW()
        ");
        return $stmt->execute([
            'chave' => $key,
            'valor' => $value,
            'valor_up' => $value
        ]);
    } catch (Exception $e) {
        return false;
    }
}

// Returns active Mercado Pago Access Token (database first, fallback to constant)
function getMercadoPagoToken() {
    $dbToken = getSystemConfig('mp_access_token', '');
    if (!empty($dbToken) && $dbToken !== 'APP_USR-SEU-ACCESS-TOKEN-AQUI') {
        return trim($dbToken);
    }
    return MP_ACCESS_TOKEN;
}

// Tests Mercado Pago credentials by calling the payment_methods endpoint
function testMercadoPagoConnection($token = null) {
    if ($token === null) {
        $token = getMercadoPagoToken();
    }
    $token = trim($token);

    if (empty($token) || $token === 'APP_USR-SEU-ACCESS-TOKEN-AQUI') {
        return [
            'success' => false,
            'message' => 'Token nao configurado ou contem o valor padrao de exemplo.'
        ];
    }

    $url = 'https://api.mercadopago.com/v1/payment_methods';
    $ch = curl_init($url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_HTTPHEADER, [
        'Authorization: Bearer ' . $token,
        'User-Agent: MasterDevSolutions-Licensing-Tester/1.0'
    ]);
    curl_setopt($ch, CURLOPT_TIMEOUT, 10);
    curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, true);

    $response = curl_exec($ch);
    $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    $error = curl_error($ch);
    curl_close($ch);

    if (!empty($error)) {
        return [
            'success' => false,
            'message' => 'Erro de conexao cURL: ' . $error
        ];
    }

    if ($httpCode === 200) {
        $data = json_decode($response, true);
        if (is_array($data) && count($data) > 0) {
            return [
                'success' => true,
                'message' => 'Conexao com Mercado Pago bem-sucedida! Credenciais validas e autorizadas.',
                'methods_count' => count($data)
            ];
        }
    }

    $errJson = json_decode($response, true);
    $detail = $errJson['message'] ?? ('HTTP ' . $httpCode . ' retornado pelo Mercado Pago.');
    return [
        'success' => false,
        'message' => 'Falha na autenticacao do Mercado Pago: ' . $detail
    ];
}

// Helper to return standardized JSON responses
function sendJsonResponse($data, $statusCode = 200) {
    http_response_code($statusCode);
    header('Content-Type: application/json; charset=utf-8');
    header('Access-Control-Allow-Origin: *');
    header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
    header('Access-Control-Allow-Headers: Content-Type, Authorization');
    echo json_encode($data, JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
    exit;
}

// Helper to generate a secure license signature token
function generateLicenseSignature($chave, $hwid, $vencimento, $status) {
    $payload = $chave . '|' . $hwid . '|' . $vencimento . '|' . $status . '|' . API_SECRET_SALT;
    return hash('sha256', $payload);
}
