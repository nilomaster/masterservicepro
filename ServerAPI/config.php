<?php
// Configuration file for MasterServicePro License and Pix Server
// All comments must use ASCII characters only.

// Database configuration
define('DB_HOST', 'localhost');
define('DB_NAME', 'masterservicepro_licencas');
define('DB_USER', 'root');
define('DB_PASS', '');

// Mercado Pago Credentials
// Insert your Production or Sandbox Access Token from Mercado Pago Developers
define('MP_ACCESS_TOKEN', 'APP_USR-SEU-ACCESS-TOKEN-AQUI');

// System settings
define('DEFAULT_MONTHLY_PRICE', 80.00);
define('API_SECRET_SALT', 'MasterServicePro_Secret_Key_2026_Salt');
define('ADMIN_PASSWORD', 'admin123'); // Change this password for production use

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
        } catch (PDOException $e) {
            header('Content-Type: application/json; charset=utf-8');
            http_response_code(500);
            echo json_encode([
                'success' => false,
                'message' => 'Database connection failed: ' . $e->getMessage()
            ]);
            exit;
        }
    }
    return $pdo;
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
