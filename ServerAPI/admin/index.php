<?php
// Admin Panel for Managing MasterDevSolutions Multi-System Licenses and Mercado Pago Pix
// All comments must use ASCII characters only.

session_start();
require_once __DIR__ . '/../config.php';

$pdo = getDbConnection();

// Dynamic configuration getters
$panelTitle = getSystemConfig('panel_title', 'MasterDevSolutions');
$adminUser = getSystemConfig('admin_name', 'niloneto');
$dbAdminPassword = getSystemConfig('admin_password', ADMIN_PASSWORD);
$currentMpToken = getSystemConfig('mp_access_token', MP_ACCESS_TOKEN);

// Handle AJAX Request: Test Mercado Pago Connection
if (isset($_GET['ajax']) && $_GET['ajax'] === 'test_mp') {
    header('Content-Type: application/json; charset=utf-8');
    if (empty($_SESSION['mds_admin_logged'])) {
        echo json_encode(['success' => false, 'message' => 'Nao autorizado']);
        exit;
    }
    $tokenToTest = trim($_POST['token'] ?? $currentMpToken);
    $result = testMercadoPagoConnection($tokenToTest);
    echo json_encode($result);
    exit;
}

// Authentication Handling
if (isset($_POST['action']) && $_POST['action'] === 'login') {
    $password = $_POST['password'] ?? '';
    if ($password === $dbAdminPassword) {
        $_SESSION['mds_admin_logged'] = true;
        header('Location: index.php');
        exit;
    } else {
        $loginError = 'Senha incorreta.';
    }
}

if (isset($_GET['logout'])) {
    session_destroy();
    header('Location: index.php');
    exit;
}

$isLogged = !empty($_SESSION['mds_admin_logged']);

// If logged in, handle admin actions
$msgSuccess = '';
$msgError = '';

if ($isLogged && $_SERVER['REQUEST_METHOD'] === 'POST') {
    $postAction = $_POST['action'] ?? '';

    // Create New License
    if ($postAction === 'create_license') {
        $sistema = trim($_POST['sistema'] ?? 'MasterServicePro');
        if (empty($sistema)) $sistema = 'MasterServicePro';
        $nome = trim($_POST['cliente_nome'] ?? '');
        $cpfCnpj = trim($_POST['cliente_cpf_cnpj'] ?? '');
        $telefone = trim($_POST['cliente_telefone'] ?? '');
        $dias = (int)($_POST['dias_validade'] ?? 30);
        $valor = (float)($_POST['valor_mensalidade'] ?? DEFAULT_MONTHLY_PRICE);

        if (empty($nome)) {
            $msgError = 'O nome do cliente é obrigatório.';
        } else {
            // Generate prefix based on system (e.g. MSP or MDS)
            $prefix = (strtoupper($sistema) === 'MASTERSERVICEPRO') ? 'MSP' : 'MDS';
            $randomPart1 = strtoupper(substr(bin2hex(random_bytes(2)), 0, 4));
            $randomPart2 = strtoupper(substr(bin2hex(random_bytes(2)), 0, 4));
            $serial = $prefix . '-' . $randomPart1 . '-' . $randomPart2;

            $vencimento = (new DateTime())->modify('+' . $dias . ' days')->format('Y-m-d H:i:s');

            $stmt = $pdo->prepare("
                INSERT INTO licencas 
                (sistema, chave_licenca, cliente_nome, cliente_cpf_cnpj, cliente_telefone, data_vencimento, status, valor_mensalidade, data_criacao)
                VALUES 
                (:sistema, :chave, :nome, :cpf, :tel, :vencimento, 'ativa', :valor, NOW())
            ");
            $stmt->execute([
                'sistema' => $sistema,
                'chave' => $serial,
                'nome' => $nome,
                'cpf' => $cpfCnpj,
                'tel' => $telefone,
                'vencimento' => $vencimento,
                'valor' => $valor
            ]);

            $msgSuccess = 'Nova licença criada com sucesso para ' . htmlspecialchars($sistema) . ': ' . $serial;
        }
    }

    // Add +30 Days
    if ($postAction === 'extend_30_days') {
        $id = (int)($_POST['licenca_id'] ?? 0);
        $stmt = $pdo->prepare('SELECT * FROM licencas WHERE id = :id LIMIT 1');
        $stmt->execute(['id' => $id]);
        $lic = $stmt->fetch();

        if ($lic) {
            $currentExp = new DateTime($lic['data_vencimento']);
            $now = new DateTime();
            $baseDate = ($now > $currentExp) ? $now : $currentExp;
            $newExp = (clone $baseDate)->modify('+30 days')->format('Y-m-d H:i:s');

            $up = $pdo->prepare("UPDATE licencas SET data_vencimento = :nova, status = 'ativa' WHERE id = :id");
            $up->execute(['nova' => $newExp, 'id' => $id]);
            $msgSuccess = 'Licença ' . $lic['chave_licenca'] . ' prorrogada por mais 30 dias!';
        }
    }

    // Toggle Block/Active
    if ($postAction === 'toggle_block') {
        $id = (int)($_POST['licenca_id'] ?? 0);
        $stmt = $pdo->prepare('SELECT * FROM licencas WHERE id = :id LIMIT 1');
        $stmt->execute(['id' => $id]);
        $lic = $stmt->fetch();

        if ($lic) {
            $newStatus = ($lic['status'] === 'bloqueada') ? 'ativa' : 'bloqueada';
            $up = $pdo->prepare('UPDATE licencas SET status = :st WHERE id = :id');
            $up->execute(['st' => $newStatus, 'id' => $id]);
            $msgSuccess = 'Status da licença ' . $lic['chave_licenca'] . ' alterado para ' . strtoupper($newStatus);
        }
    }

    // Reset Hardware ID (HWID)
    if ($postAction === 'reset_hwid') {
        $id = (int)($_POST['licenca_id'] ?? 0);
        $up = $pdo->prepare('UPDATE licencas SET hwid = NULL WHERE id = :id');
        $up->execute(['id' => $id]);
        $msgSuccess = 'Vínculo de máquina (HWID) resetado com sucesso! A licença poderá ser ativada em outro PC.';
    }

    // Delete License
    if ($postAction === 'delete_license') {
        $id = (int)($_POST['licenca_id'] ?? 0);
        $stmt = $pdo->prepare('SELECT chave_licenca FROM licencas WHERE id = :id LIMIT 1');
        $stmt->execute(['id' => $id]);
        $lic = $stmt->fetch();
        if ($lic) {
            $del = $pdo->prepare('DELETE FROM licencas WHERE id = :id');
            $del->execute(['id' => $id]);
            $msgSuccess = 'Licença ' . $lic['chave_licenca'] . ' excluída com sucesso!';
        }
    }

    // Save Settings & Mercado Pago Credentials
    if ($postAction === 'save_settings') {
        $newMpToken = trim($_POST['mp_access_token'] ?? '');
        $newAdminName = trim($_POST['admin_name'] ?? 'niloneto');
        $newAdminPass = trim($_POST['admin_password'] ?? '');

        if (!empty($newMpToken)) {
            setSystemConfig('mp_access_token', $newMpToken);
            $currentMpToken = $newMpToken;
        }
        if (!empty($newAdminName)) {
            setSystemConfig('admin_name', $newAdminName);
            $adminUser = $newAdminName;
        }
        if (!empty($newAdminPass)) {
            setSystemConfig('admin_password', $newAdminPass);
            $dbAdminPassword = $newAdminPass;
        }

        $msgSuccess = 'Configurações e credenciais salvas com sucesso!';
    }
}

// Fetch dashboard data if logged in
$licencas = [];
$pagamentos = [];
$licencasExpiringSoon = [];
$totalLicencas = 0;
$totalAtivas = 0;
$totalVencidas = 0;
$totalExpiringSoon = 0;
$totalRecebido = 0.0;
$totalDevices = 0;
$sistemasDisponiveis = [];

if ($isLogged) {
    $licencas = $pdo->query('SELECT * FROM licencas ORDER BY id DESC')->fetchAll();
    $pagamentos = $pdo->query('SELECT p.*, l.cliente_nome, l.sistema FROM pagamentos_pix p JOIN licencas l ON p.id_licenca = l.id ORDER BY p.id DESC LIMIT 50')->fetchAll();

    $totalLicencas = count($licencas);
    $now = new DateTime();
    $fiveDaysLater = (clone $now)->modify('+5 days');

    foreach ($licencas as $l) {
        $venc = new DateTime($l['data_vencimento']);
        $sistemaName = !empty($l['sistema']) ? $l['sistema'] : 'MasterServicePro';
        if (!in_array($sistemaName, $sistemasDisponiveis)) {
            $sistemasDisponiveis[] = $sistemaName;
        }

        if (!empty($l['hwid'])) {
            $totalDevices++;
        }

        if ($l['status'] === 'bloqueada') {
            $totalVencidas++;
        } else if ($now > $venc) {
            $totalVencidas++;
        } else {
            $totalAtivas++;
            if ($venc <= $fiveDaysLater) {
                $totalExpiringSoon++;
                $licencasExpiringSoon[] = $l;
            }
        }
    }

    $sumStmt = $pdo->query("SELECT SUM(valor) as total FROM pagamentos_pix WHERE status = 'approved'");
    $sumRow = $sumStmt->fetch();
    $totalRecebido = (float)($sumRow['total'] ?? 0.0);
}

// Base Webhook URL
$scheme = (!empty($_SERVER['HTTPS']) && $_SERVER['HTTPS'] !== 'off') ? 'https' : 'http';
$host = $_SERVER['HTTP_HOST'] ?? 'localhost';
$currentPath = dirname(dirname($_SERVER['SCRIPT_NAME']));
$webhookUrl = $scheme . '://' . $host . rtrim($currentPath, '/') . '/api/webhook.php';
?>
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title><?= htmlspecialchars($panelTitle) ?> - Painel de Gestão</title>
    <!-- Google Fonts Inter -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&display=swap" rel="stylesheet">
    <!-- Phosphor Icons -->
    <script src="https://unpkg.com/@phosphor-icons/web"></script>
    <style>
        /* Modern Clean SaaS Dashboard Design */
        /* All CSS comments must use ASCII characters only */
        :root {
            --sidebar-bg: #1E222D;
            --sidebar-active: #0D6EFD;
            --sidebar-text: #A6B0CF;
            --sidebar-header: #FFFFFF;
            --body-bg: #F4F6F9;
            --card-bg: #FFFFFF;
            --text-main: #33383F;
            --text-muted: #74788D;
            --border-color: #E9ECEF;
            --primary: #0D6EFD;
            --primary-hover: #0B5ED7;
            --success: #28A745;
            --danger: #DC3545;
            --warning: #FFC107;
            --info: #17A2B8;
            --teal: #20C997;
            --purple: #6F42C1;
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
        }

        body {
            background-color: var(--body-bg);
            color: var(--text-main);
            min-height: 100vh;
            display: flex;
        }

        /* Layout Container */
        .app-wrapper {
            display: flex;
            width: 100%;
            min-height: 100vh;
        }

        /* Sidebar Styling (Matches Android Multi Tool visual) */
        .sidebar {
            width: 250px;
            background-color: var(--sidebar-bg);
            color: var(--sidebar-text);
            display: flex;
            flex-direction: column;
            flex-shrink: 0;
            position: fixed;
            top: 0;
            bottom: 0;
            left: 0;
            z-index: 100;
            transition: all 0.3s ease;
        }

        .sidebar-brand {
            height: 65px;
            display: flex;
            align-items: center;
            padding: 0 20px;
            background: rgba(0, 0, 0, 0.15);
            font-size: 16px;
            font-weight: 700;
            color: #FFFFFF;
            letter-spacing: -0.2px;
            gap: 10px;
        }

        .sidebar-brand i {
            font-size: 24px;
            color: #38BDF8;
        }

        /* User Profile in Sidebar */
        .sidebar-user {
            display: flex;
            align-items: center;
            padding: 20px;
            gap: 12px;
            border-bottom: 1px solid rgba(255, 255, 255, 0.06);
        }

        .user-avatar {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background: linear-gradient(135deg, #0D6EFD, #6F42C1);
            color: #FFF;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 700;
            font-size: 16px;
            border: 2px solid rgba(255, 255, 255, 0.2);
        }

        .user-info .user-name {
            font-size: 14px;
            font-weight: 600;
            color: #FFFFFF;
        }

        .user-info .user-role {
            font-size: 11px;
            color: #20C997;
            display: flex;
            align-items: center;
            gap: 4px;
        }

        .user-info .user-role::before {
            content: '';
            width: 6px;
            height: 6px;
            background: #20C997;
            border-radius: 50%;
            display: inline-block;
        }

        /* Sidebar Navigation */
        .sidebar-nav {
            flex: 1;
            padding: 15px 0;
            overflow-y: auto;
        }

        .nav-category {
            font-size: 11px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            color: #6C757D;
            padding: 12px 20px 6px 20px;
            font-weight: 700;
        }

        .nav-item {
            display: flex;
            align-items: center;
            padding: 12px 20px;
            color: var(--sidebar-text);
            text-decoration: none;
            font-size: 13px;
            font-weight: 500;
            gap: 12px;
            transition: all 0.2s ease;
            cursor: pointer;
        }

        .nav-item i {
            font-size: 18px;
        }

        .nav-item:hover {
            color: #FFFFFF;
            background: rgba(255, 255, 255, 0.04);
        }

        .nav-item.active {
            color: #FFFFFF;
            background-color: var(--sidebar-active);
            border-radius: 0 4px 4px 0;
            margin-right: 15px;
        }

        .nav-badge {
            margin-left: auto;
            background: #EF4444;
            color: #FFF;
            font-size: 11px;
            font-weight: 700;
            padding: 2px 7px;
            border-radius: 10px;
        }

        .sidebar-footer {
            padding: 15px 20px;
            font-size: 11px;
            color: #6C757D;
            border-top: 1px solid rgba(255, 255, 255, 0.05);
            text-align: center;
        }

        /* Main Content Area */
        .main-content {
            margin-left: 250px;
            flex: 1;
            display: flex;
            flex-direction: column;
            min-height: 100vh;
        }

        /* Topbar Header */
        .topbar {
            height: 65px;
            background: #FFFFFF;
            border-bottom: 1px solid var(--border-color);
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 0 30px;
            position: sticky;
            top: 0;
            z-index: 90;
        }

        .topbar-left {
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .topbar-title {
            font-size: 18px;
            font-weight: 700;
            color: var(--text-main);
        }

        .breadcrumbs {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 13px;
            color: var(--text-muted);
        }

        .breadcrumbs a {
            color: var(--primary);
            text-decoration: none;
        }

        .btn-logout {
            background: #0D6EFD;
            color: #FFFFFF;
            padding: 8px 16px;
            border-radius: 6px;
            font-size: 13px;
            font-weight: 600;
            text-decoration: none;
            display: flex;
            align-items: center;
            gap: 6px;
            transition: 0.2s;
        }

        .btn-logout:hover {
            background: #0B5ED7;
        }

        /* Content Body */
        .content-body {
            padding: 25px 30px;
            flex: 1;
        }

        /* 5 Stats Cards (Matches user screenshot) */
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(210px, 1fr));
            gap: 20px;
            margin-bottom: 25px;
        }

        .stat-card {
            background: #FFFFFF;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.04);
            border: 1px solid var(--border-color);
            display: flex;
            align-items: center;
            padding: 16px;
            gap: 16px;
        }

        .stat-icon {
            width: 54px;
            height: 54px;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 26px;
            color: #FFFFFF;
            flex-shrink: 0;
        }

        .stat-icon.green { background-color: #28A745; }
        .stat-icon.red { background-color: #DC3545; }
        .stat-icon.orange { background-color: #FFC107; }
        .stat-icon.cyan { background-color: #17A2B8; }
        .stat-icon.yellow { background-color: #FFB300; }
        .stat-icon.blue { background-color: #0D6EFD; }

        .stat-details .stat-label {
            font-size: 12px;
            color: var(--text-muted);
            font-weight: 600;
            margin-bottom: 4px;
        }

        .stat-details .stat-value {
            font-size: 22px;
            font-weight: 800;
            color: var(--text-main);
        }

        /* Filter & Search Bar */
        .filter-card {
            background: #FFFFFF;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            padding: 18px 20px;
            margin-bottom: 25px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.02);
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 15px;
            flex-wrap: wrap;
        }

        .search-box {
            position: relative;
            flex: 1;
            min-width: 260px;
        }

        .search-box i {
            position: absolute;
            left: 14px;
            top: 50%;
            transform: translateY(-50%);
            color: #9CA3AF;
            font-size: 16px;
        }

        .search-input {
            width: 100%;
            padding: 10px 14px 10px 40px;
            border: 1px solid #D1D5DB;
            border-radius: 6px;
            font-size: 13px;
            outline: none;
            transition: 0.2s;
        }

        .search-input:focus {
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(13, 110, 253, 0.15);
        }

        .filter-controls {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
        }

        .filter-select {
            padding: 9px 12px;
            border: 1px solid #D1D5DB;
            border-radius: 6px;
            font-size: 13px;
            background: #FFFFFF;
            outline: none;
            color: var(--text-main);
        }

        .btn-new-license {
            background: #0D6EFD;
            color: #FFF;
            padding: 9px 16px;
            border-radius: 6px;
            border: none;
            font-size: 13px;
            font-weight: 600;
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 6px;
            transition: 0.2s;
        }

        .btn-new-license:hover {
            background: #0B5ED7;
        }

        /* Section Container */
        .card {
            background: #FFFFFF;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.03);
            margin-bottom: 25px;
        }

        .card-header {
            padding: 18px 22px;
            border-bottom: 1px solid var(--border-color);
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .card-header h2 {
            font-size: 15px;
            font-weight: 700;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .card-body {
            padding: 22px;
        }

        /* Table Styling */
        .table-responsive {
            overflow-x: auto;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            font-size: 13px;
            text-align: left;
        }

        th {
            background: #F8F9FA;
            color: #495057;
            padding: 12px 16px;
            font-weight: 600;
            border-bottom: 1px solid var(--border-color);
        }

        td {
            padding: 13px 16px;
            border-bottom: 1px solid var(--border-color);
            vertical-align: middle;
        }

        tr:hover td {
            background-color: #F8FAFC;
        }

        .serial-tag {
            background: #EFF6FF;
            color: #1D4ED8;
            font-family: monospace;
            font-size: 13px;
            font-weight: 600;
            padding: 4px 8px;
            border-radius: 4px;
            border: 1px solid #BFDBFE;
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

        .btn-copy-serial {
            background: transparent;
            border: none;
            color: #3B82F6;
            cursor: pointer;
            padding: 0;
            font-size: 14px;
        }

        .system-badge {
            background: #F3F4F6;
            color: #374151;
            font-size: 11px;
            font-weight: 600;
            padding: 3px 8px;
            border-radius: 4px;
            border: 1px solid #E5E7EB;
        }

        .badge-status {
            padding: 4px 10px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 600;
            display: inline-block;
        }

        .status-active { background: #D1FAE5; color: #065F46; }
        .status-expired { background: #FEE2E2; color: #991B1B; }
        .status-warning { background: #FEF3C7; color: #92400E; }
        .status-blocked { background: #E5E7EB; color: #374151; }

        /* Action Buttons */
        .btn-action {
            padding: 5px 9px;
            border-radius: 4px;
            border: 1px solid transparent;
            font-size: 11px;
            font-weight: 600;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            transition: 0.15s;
        }

        .btn-act-extend { background: #EFF6FF; color: #1D4ED8; border-color: #BFDBFE; }
        .btn-act-extend:hover { background: #DBEAFE; }

        .btn-act-block { background: #F3F4F6; color: #4B5563; border-color: #E5E7EB; }
        .btn-act-block:hover { background: #E5E7EB; }

        .btn-act-reset { background: #FEF2F2; color: #DC2626; border-color: #FECACA; }
        .btn-act-reset:hover { background: #FEE2E2; }

        .btn-act-wa { background: #DCFCE7; color: #15803D; border-color: #BBF7D0; text-decoration: none; }
        .btn-act-wa:hover { background: #BBF7D0; }

        .btn-act-del { background: #FFF1F2; color: #BE123C; border-color: #FFE4E6; }
        .btn-act-del:hover { background: #FFE4E6; }

        /* Expiring Soon Alert Banner / Table */
        .alert-expiring-box {
            background: #FFFBEB;
            border: 1px solid #FDE68A;
            border-radius: 8px;
            padding: 16px 20px;
            margin-bottom: 25px;
        }

        .alert-expiring-box h3 {
            font-size: 14px;
            font-weight: 700;
            color: #92400E;
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 6px;
        }

        .alert-expiring-box p {
            font-size: 12px;
            color: #B45309;
        }

        /* Modal Styling */
        .modal {
            display: none;
            position: fixed;
            inset: 0;
            background: rgba(0, 0, 0, 0.5);
            backdrop-filter: blur(2px);
            z-index: 200;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .modal.active {
            display: flex;
        }

        .modal-card {
            background: #FFFFFF;
            border-radius: 10px;
            width: 100%;
            max-width: 580px;
            box-shadow: 0 15px 35px rgba(0, 0, 0, 0.2);
            overflow: hidden;
            animation: modalFadeIn 0.2s ease forwards;
        }

        @keyframes modalFadeIn {
            from { opacity: 0; transform: translateY(15px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .modal-header {
            padding: 18px 24px;
            border-bottom: 1px solid var(--border-color);
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .modal-header h3 {
            font-size: 16px;
            font-weight: 700;
        }

        .btn-close-modal {
            background: transparent;
            border: none;
            font-size: 20px;
            color: #9CA3AF;
            cursor: pointer;
        }

        .btn-close-modal:hover { color: #111; }

        .modal-body {
            padding: 24px;
        }

        .form-group {
            margin-bottom: 16px;
        }

        .form-group label {
            display: block;
            font-size: 12px;
            font-weight: 600;
            color: #4B5563;
            margin-bottom: 6px;
        }

        .form-control {
            width: 100%;
            padding: 10px 14px;
            border: 1px solid #D1D5DB;
            border-radius: 6px;
            font-size: 13px;
            outline: none;
        }

        .form-control:focus {
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(13, 110, 253, 0.15);
        }

        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 15px;
        }

        .modal-footer {
            padding: 16px 24px;
            background: #F9FAFB;
            border-top: 1px solid var(--border-color);
            display: flex;
            justify-content: flex-end;
            gap: 10px;
        }

        .btn-secondary {
            background: #E5E7EB;
            color: #374151;
            border: none;
            padding: 9px 16px;
            border-radius: 6px;
            font-size: 13px;
            font-weight: 600;
            cursor: pointer;
        }

        .btn-submit {
            background: #0D6EFD;
            color: #FFFFFF;
            border: none;
            padding: 9px 18px;
            border-radius: 6px;
            font-size: 13px;
            font-weight: 600;
            cursor: pointer;
        }

        .btn-submit:hover { background: #0B5ED7; }

        /* Alert Toast & Feedback */
        .alert-banner {
            padding: 12px 18px;
            border-radius: 6px;
            font-size: 13px;
            font-weight: 500;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .alert-success { background: #D1FAE5; color: #065F46; border: 1px solid #A7F3D0; }
        .alert-danger { background: #FEE2E2; color: #991B1B; border: 1px solid #FECACA; }

        /* Login Layout */
        .login-wrapper {
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 100vh;
            width: 100%;
            background: #0F172A;
        }

        .login-card {
            background: #1E293B;
            border: 1px solid #334155;
            border-radius: 12px;
            padding: 40px;
            width: 100%;
            max-width: 400px;
            color: #FFF;
            box-shadow: 0 20px 40px rgba(0, 0, 0, 0.5);
            text-align: center;
        }

        .login-card h2 {
            font-size: 22px;
            font-weight: 700;
            margin-bottom: 6px;
        }

        .login-card h2 span { color: #38BDF8; }

        .login-card p {
            color: #94A3B8;
            font-size: 13px;
            margin-bottom: 25px;
        }

        /* View sections */
        .view-tab {
            display: none;
        }

        .view-tab.active {
            display: block;
        }
    </style>
</head>
<body>

<?php if (!$isLogged): ?>
    <!-- Login Screen -->
    <div class="login-wrapper">
        <div class="login-card">
            <div style="font-size: 44px; color: #38BDF8; margin-bottom: 10px;">
                <i class="ph-bold ph-shield-check"></i>
            </div>
            <h2>MasterDev<span>Solutions</span></h2>
            <p>Painel de Gestão e Controle de Licenças</p>

            <?php if (!empty($loginError)): ?>
                <div class="alert-banner alert-danger" style="margin-bottom: 20px; font-size: 12px;">
                    <i class="ph-bold ph-warning-circle"></i> <?= htmlspecialchars($loginError) ?>
                </div>
            <?php endif; ?>

            <form method="POST">
                <input type="hidden" name="action" value="login">
                <div class="form-group" style="text-align: left;">
                    <label style="color: #94A3B8;">Senha de Acesso:</label>
                    <input type="password" name="password" class="form-control" style="background: #0F172A; border-color: #334155; color: #FFF;" placeholder="Digite sua senha de administrador" required autofocus>
                </div>
                <button type="submit" class="btn-submit" style="width: 100%; padding: 12px; font-size: 14px; background: #0D6EFD; margin-top: 10px;">Entrar no Painel</button>
            </form>
            <div style="margin-top: 25px; font-size: 11px; color: #64748B;">
                MasterDevSolutions Licensing System v2.0
            </div>
        </div>
    </div>
<?php else: ?>
    <!-- Main Authenticated Layout -->
    <div class="app-wrapper">

        <!-- Sidebar Navigation -->
        <aside class="sidebar">
            <div class="sidebar-brand">
                <i class="ph-bold ph-cpu"></i>
                <span>MasterDevSolutions</span>
            </div>

            <!-- Admin Profile -->
            <div class="sidebar-user">
                <div class="user-avatar">
                    <?= strtoupper(substr($adminUser, 0, 1)) ?>
                </div>
                <div class="user-info">
                    <div class="user-name"><?= htmlspecialchars($adminUser) ?></div>
                    <div class="user-role">Super Admin</div>
                </div>
            </div>

            <nav class="sidebar-nav">
                <div class="nav-category">VISÃO GERAL</div>
                <a class="nav-item active" data-tab="tab-dashboard" onclick="switchTab('tab-dashboard', this)">
                    <i class="ph-bold ph-gauge"></i>
                    <span>Dashboard</span>
                </a>

                <div class="nav-category">GESTÃO DE LICENÇAS</div>
                <a class="nav-item" data-tab="tab-licencas" onclick="switchTab('tab-licencas', this)">
                    <i class="ph-bold ph-key"></i>
                    <span>Todas as Licenças</span>
                </a>
                <a class="nav-item" data-tab="tab-expiring" onclick="switchTab('tab-expiring', this)">
                    <i class="ph-bold ph-clock-countdown"></i>
                    <span>Vencendo em Breve</span>
                    <?php if ($totalExpiringSoon > 0): ?>
                        <span class="nav-badge"><?= $totalExpiringSoon ?></span>
                    <?php endif; ?>
                </a>
                <a class="nav-item" onclick="openNewLicenseModal()">
                    <i class="ph-bold ph-plus-circle"></i>
                    <span>Nova Licença</span>
                </a>

                <div class="nav-category">INTEGRAÇÕES & SERVIÇOS</div>
                <a class="nav-item" data-tab="tab-mercadopago" onclick="switchTab('tab-mercadopago', this)">
                    <i class="ph-bold ph-qr-code"></i>
                    <span>API Mercado Pago</span>
                </a>
                <a class="nav-item" data-tab="tab-pix-history" onclick="switchTab('tab-pix-history', this)">
                    <i class="ph-bold ph-receipt"></i>
                    <span>Histórico Pix</span>
                </a>
                <a class="nav-item" data-tab="tab-settings" onclick="switchTab('tab-settings', this)">
                    <i class="ph-bold ph-gear"></i>
                    <span>Configurações</span>
                </a>
            </nav>

            <div class="sidebar-footer">
                Developed by <strong>MasterDev Team</strong>
            </div>
        </aside>

        <!-- Main Content Area -->
        <main class="main-content">
            <!-- Topbar -->
            <header class="topbar">
                <div class="topbar-left">
                    <h1 class="topbar-title" id="pageTitle">Dashboard</h1>
                    <div class="breadcrumbs">
                        <span>/</span>
                        <a href="javascript:void(0)">Home</a>
                        <span>/</span>
                        <span id="breadcrumbCurrent">Dashboard</span>
                    </div>
                </div>
                <div>
                    <a href="?logout=1" class="btn-logout" title="Encerrar Sessão">
                        <i class="ph-bold ph-sign-out"></i>
                        <span>Logout</span>
                    </a>
                </div>
            </header>

            <div class="content-body">
                <!-- Flash Messages -->
                <?php if (!empty($msgSuccess)): ?>
                    <div class="alert-banner alert-success">
                        <i class="ph-bold ph-check-circle" style="font-size: 18px;"></i>
                        <span><?= htmlspecialchars($msgSuccess) ?></span>
                    </div>
                <?php endif; ?>
                <?php if (!empty($msgError)): ?>
                    <div class="alert-banner alert-danger">
                        <i class="ph-bold ph-warning-circle" style="font-size: 18px;"></i>
                        <span><?= htmlspecialchars($msgError) ?></span>
                    </div>
                <?php endif; ?>

                <!-- TAB 1: DASHBOARD (OVERVIEW) -->
                <div class="view-tab active" id="tab-dashboard">
                    <!-- 5 Stats Cards (Matches User Reference) -->
                    <div class="stats-grid">
                        <!-- Card 1: Licenças Ativas -->
                        <div class="stat-card">
                            <div class="stat-icon green">
                                <i class="ph-bold ph-lock-key-open"></i>
                            </div>
                            <div class="stat-details">
                                <div class="stat-label">Licenças Ativas</div>
                                <div class="stat-value"><?= $totalAtivas ?></div>
                            </div>
                        </div>

                        <!-- Card 2: Licenças Vencidas -->
                        <div class="stat-card">
                            <div class="stat-icon red">
                                <i class="ph-bold ph-warning-octagon"></i>
                            </div>
                            <div class="stat-details">
                                <div class="stat-label">Licenças Vencidas</div>
                                <div class="stat-value"><?= $totalVencidas ?></div>
                            </div>
                        </div>

                        <!-- Card 3: Vencendo em 5 dias -->
                        <div class="stat-card">
                            <div class="stat-icon orange">
                                <i class="ph-bold ph-hourglass-medium"></i>
                            </div>
                            <div class="stat-details">
                                <div class="stat-label">Vencem em 5 Dias</div>
                                <div class="stat-value"><?= $totalExpiringSoon ?></div>
                            </div>
                        </div>

                        <!-- Card 4: Faturamento Pix Total -->
                        <div class="stat-card">
                            <div class="stat-icon blue">
                                <i class="ph-bold ph-currency-dollar"></i>
                            </div>
                            <div class="stat-details">
                                <div class="stat-label">Total Recebido (Pix)</div>
                                <div class="stat-value">R$ <?= number_format($totalRecebido, 2, ',', '.') ?></div>
                            </div>
                        </div>

                        <!-- Card 5: Dispositivos Vinculados (HWID) -->
                        <div class="stat-card">
                            <div class="stat-icon cyan">
                                <i class="ph-bold ph-desktop"></i>
                            </div>
                            <div class="stat-details">
                                <div class="stat-label">Dispositivos HWID</div>
                                <div class="stat-value"><?= $totalDevices ?></div>
                            </div>
                        </div>
                    </div>

                    <?php if ($totalExpiringSoon > 0): ?>
                        <!-- Expiring Soon Warning Banner -->
                        <div class="alert-expiring-box">
                            <h3><i class="ph-bold ph-bell-ringing"></i> Atenção: <?= $totalExpiringSoon ?> cliente(s) vencendo nos próximos 5 dias!</h3>
                            <p>Envie o lembrete de cobrança preventiva via WhatsApp para evitar o bloqueio automático dos sistemas.</p>
                        </div>
                    <?php endif; ?>

                    <!-- Filter & Search Toolbar -->
                    <div class="filter-card">
                        <div class="search-box">
                            <i class="ph-bold ph-magnifying-glass"></i>
                            <input type="text" id="txtSearchLicenses" class="search-input" placeholder="Pesquisar por Cliente, CPF/CNPJ, Telefone ou Serial..." oninput="filterLicensesTable()">
                        </div>
                        <div class="filter-controls">
                            <select id="selFilterSystem" class="filter-select" onchange="filterLicensesTable()">
                                <option value="">Todos os Sistemas</option>
                                <?php foreach ($sistemasDisponiveis as $sis): ?>
                                    <option value="<?= htmlspecialchars($sis) ?>"><?= htmlspecialchars($sis) ?></option>
                                <?php endforeach; ?>
                            </select>

                            <select id="selFilterStatus" class="filter-select" onchange="filterLicensesTable()">
                                <option value="">Todos os Status</option>
                                <option value="ativa">Ativas</option>
                                <option value="vencida">Vencidas</option>
                                <option value="bloqueada">Bloqueadas</option>
                            </select>

                            <button class="btn-new-license" onclick="openNewLicenseModal()">
                                <i class="ph-bold ph-plus"></i>
                                <span>Nova Licença</span>
                            </button>
                        </div>
                    </div>

                    <!-- Main Licenses Table -->
                    <div class="card">
                        <div class="card-header">
                            <h2><i class="ph-bold ph-table"></i> Gestão de Licenças e Clientes</h2>
                            <span style="font-size: 12px; color: var(--text-muted);" id="lblLicensesCount">Exibindo <?= count($licencas) ?> licença(s)</span>
                        </div>
                        <div class="table-responsive">
                            <table id="tblLicenses">
                                <thead>
                                    <tr>
                                        <th>Sistema</th>
                                        <th>Serial / Chave</th>
                                        <th>Cliente</th>
                                        <th>Contato</th>
                                        <th>HWID (Máquina)</th>
                                        <th>Vencimento</th>
                                        <th>Status</th>
                                        <th>Mensalidade</th>
                                        <th style="text-align: right;">Ações Rápidas</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <?php if (empty($licencas)): ?>
                                        <tr class="row-no-data"><td colspan="9" style="text-align: center; color: var(--text-muted); padding: 35px;">Nenhuma licença cadastrada até o momento.</td></tr>
                                    <?php else: ?>
                                        <?php foreach ($licencas as $lic): 
                                            $now = new DateTime();
                                            $venc = new DateTime($lic['data_vencimento']);
                                            $diff = $now->diff($venc);
                                            $isVencida = ($now > $venc);
                                            $diasRestantes = (int)$diff->format('%r%a');

                                            $statusKey = 'ativa';
                                            $badgeClass = 'status-active';
                                            $statusLabel = 'Ativa';

                                            if ($lic['status'] === 'bloqueada') {
                                                $statusKey = 'bloqueada';
                                                $badgeClass = 'status-blocked';
                                                $statusLabel = 'Bloqueada';
                                            } else if ($isVencida || $lic['status'] === 'vencida') {
                                                $statusKey = 'vencida';
                                                $badgeClass = 'status-expired';
                                                $statusLabel = 'Vencida';
                                            } else if ($diasRestantes <= 5) {
                                                $badgeClass = 'status-warning';
                                                $statusLabel = 'Vence em ' . $diasRestantes . 'd';
                                            }

                                            // WhatsApp message generator
                                            $phoneRaw = preg_replace('/[^0-9]/', '', $lic['cliente_telefone'] ?? '');
                                            $waLink = '';
                                            if (!empty($phoneRaw)) {
                                                if (strlen($phoneRaw) <= 11) $phoneRaw = '55' . $phoneRaw;
                                                $msgWa = "Ola " . $lic['cliente_nome'] . "! Mensalidade do sistema " . ($lic['sistema'] ?? 'MasterServicePro') . " referente a chave " . $lic['chave_licenca'] . " vence em " . $venc->format('d/m/Y') . ". Para renovar ou tirar duvidas, estamos a disposicao!";
                                                $waLink = 'https://wa.me/' . $phoneRaw . '?text=' . rawurlencode($msgWa);
                                            }
                                        ?>
                                            <tr class="license-row" 
                                                data-search="<?= htmlspecialchars(strtolower($lic['cliente_nome'] . ' ' . $lic['cliente_cpf_cnpj'] . ' ' . $lic['cliente_telefone'] . ' ' . $lic['chave_licenca'] . ' ' . $lic['sistema'])) ?>"
                                                data-system="<?= htmlspecialchars($lic['sistema'] ?? 'MasterServicePro') ?>"
                                                data-status="<?= $statusKey ?>">
                                                <td><span class="system-badge"><?= htmlspecialchars($lic['sistema'] ?? 'MasterServicePro') ?></span></td>
                                                <td>
                                                    <span class="serial-tag">
                                                        <span><?= htmlspecialchars($lic['chave_licenca']) ?></span>
                                                        <button class="btn-copy-serial" onclick="copyText('<?= htmlspecialchars($lic['chave_licenca']) ?>', 'Serial')" title="Copiar Chave"><i class="ph-bold ph-copy"></i></button>
                                                    </span>
                                                </td>
                                                <td><strong><?= htmlspecialchars($lic['cliente_nome']) ?></strong></td>
                                                <td>
                                                    <?php if (!empty($waLink)): ?>
                                                        <a href="<?= $waLink ?>" target="_blank" class="btn-action btn-act-wa" title="Conversar no WhatsApp">
                                                            <i class="ph-bold ph-whatsapp-logo"></i> <?= htmlspecialchars($lic['cliente_telefone']) ?>
                                                        </a>
                                                    <?php else: ?>
                                                        <span style="color: var(--text-muted);"><?= htmlspecialchars($lic['cliente_telefone'] ?? '-') ?></span>
                                                    <?php endif; ?>
                                                </td>
                                                <td style="font-size: 11px; color: var(--text-muted);">
                                                    <?php if (!empty($lic['hwid'])): ?>
                                                        <span title="<?= htmlspecialchars($lic['hwid']) ?>" style="cursor:help;">
                                                            <i class="ph-bold ph-desktop" style="color:#0D6EFD;"></i> <?= htmlspecialchars(substr($lic['hwid'], 0, 12)) ?>...
                                                        </span>
                                                    <?php else: ?>
                                                        <span style="color: #9CA3AF;"><i class="ph-bold ph-circle-dashed"></i> Não ativada</span>
                                                    <?php endif; ?>
                                                </td>
                                                <td>
                                                    <strong><?= $venc->format('d/m/Y') ?></strong>
                                                    <div style="font-size: 11px; color: var(--text-muted);">
                                                        <?= ($isVencida) ? '<span style="color:#DC2626;">Venceu há ' . abs($diasRestantes) . ' dia(s)</span>' : 'Faltam ' . $diasRestantes . ' dia(s)' ?>
                                                    </div>
                                                </td>
                                                <td><span class="badge-status <?= $badgeClass ?>"><?= $statusLabel ?></span></td>
                                                <td><strong>R$ <?= number_format($lic['valor_mensalidade'], 2, ',', '.') ?></strong></td>
                                                <td style="text-align: right; white-space: nowrap;">
                                                    <form method="POST" style="display:inline;">
                                                        <input type="hidden" name="action" value="extend_30_days">
                                                        <input type="hidden" name="licenca_id" value="<?= $lic['id'] ?>">
                                                        <button type="submit" class="btn-action btn-act-extend" title="Prorrogar por +30 dias">+30d</button>
                                                    </form>
                                                    <form method="POST" style="display:inline;">
                                                        <input type="hidden" name="action" value="toggle_block">
                                                        <input type="hidden" name="licenca_id" value="<?= $lic['id'] ?>">
                                                        <button type="submit" class="btn-action btn-act-block" title="Bloquear ou Desbloquear"><?= $lic['status'] === 'bloqueada' ? 'Desbloquear' : 'Bloquear' ?></button>
                                                    </form>
                                                    <?php if (!empty($lic['hwid'])): ?>
                                                        <form method="POST" style="display:inline;" onsubmit="return confirm('Resetar o HWID permitirá ativar esta licença em outro computador. Confirmar?');">
                                                            <input type="hidden" name="action" value="reset_hwid">
                                                            <input type="hidden" name="licenca_id" value="<?= $lic['id'] ?>">
                                                            <button type="submit" class="btn-action btn-act-reset" title="Desvincular HWID do Computador">Reset HWID</button>
                                                        </form>
                                                    <?php endif; ?>
                                                    <form method="POST" style="display:inline;" onsubmit="return confirm('Excluir esta licença permanentemente?');">
                                                        <input type="hidden" name="action" value="delete_license">
                                                        <input type="hidden" name="licenca_id" value="<?= $lic['id'] ?>">
                                                        <button type="submit" class="btn-action btn-act-del" title="Excluir Licença"><i class="ph-bold ph-trash"></i></button>
                                                    </form>
                                                </td>
                                            </tr>
                                        <?php endforeach; ?>
                                    <?php endif; ?>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>

                <!-- TAB 2: VENCENDO EM BREVE -->
                <div class="view-tab" id="tab-expiring">
                    <div class="card">
                        <div class="card-header">
                            <h2><i class="ph-bold ph-clock-countdown" style="color: #D97706;"></i> Clientes Vencendo nos Próximos 5 Dias</h2>
                            <span style="font-size: 12px; color: var(--text-muted);"><?= count($licencasExpiringSoon) ?> cliente(s)</span>
                        </div>
                        <div class="table-responsive">
                            <table>
                                <thead>
                                    <tr>
                                        <th>Sistema</th>
                                        <th>Cliente</th>
                                        <th>Contato / WhatsApp</th>
                                        <th>Serial</th>
                                        <th>Vencimento</th>
                                        <th>Dias Restantes</th>
                                        <th>Valor</th>
                                        <th>Ação Recomendada</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <?php if (empty($licencasExpiringSoon)): ?>
                                        <tr><td colspan="8" style="text-align: center; color: var(--text-muted); padding: 35px;">Nenhuma licença vencendo nos próximos 5 dias! Todos os clientes em dia.</td></tr>
                                    <?php else: ?>
                                        <?php foreach ($licencasExpiringSoon as $lic): 
                                            $venc = new DateTime($lic['data_vencimento']);
                                            $diff = (new DateTime())->diff($venc);
                                            $diasRestantes = (int)$diff->format('%r%a');
                                            $phoneRaw = preg_replace('/[^0-9]/', '', $lic['cliente_telefone'] ?? '');
                                            if (strlen($phoneRaw) <= 11 && !empty($phoneRaw)) $phoneRaw = '55' . $phoneRaw;
                                            $msgWa = "Ola " . $lic['cliente_nome'] . "! Passando para avisar que sua mensalidade do " . ($lic['sistema'] ?? 'MasterServicePro') . " vence em " . $venc->format('d/m/Y') . " (R$ " . number_format($lic['valor_mensalidade'], 2, ',', '.') . "). O sistema foi emitido sob a chave " . $lic['chave_licenca'] . ". Qualquer duvida estamos a disposicao!";
                                            $waLink = !empty($phoneRaw) ? 'https://wa.me/' . $phoneRaw . '?text=' . rawurlencode($msgWa) : '';
                                        ?>
                                            <tr>
                                                <td><span class="system-badge"><?= htmlspecialchars($lic['sistema'] ?? 'MasterServicePro') ?></span></td>
                                                <td><strong><?= htmlspecialchars($lic['cliente_nome']) ?></strong></td>
                                                <td><?= htmlspecialchars($lic['cliente_telefone'] ?? '-') ?></td>
                                                <td><span class="serial-tag"><?= htmlspecialchars($lic['chave_licenca']) ?></span></td>
                                                <td><strong><?= $venc->format('d/m/Y') ?></strong></td>
                                                <td><span class="badge-status status-warning"><?= $diasRestantes ?> dia(s)</span></td>
                                                <td>R$ <?= number_format($lic['valor_mensalidade'], 2, ',', '.') ?></td>
                                                <td>
                                                    <?php if (!empty($waLink)): ?>
                                                        <a href="<?= $waLink ?>" target="_blank" class="btn-action btn-act-wa" style="padding: 7px 12px; font-size: 12px;">
                                                            <i class="ph-bold ph-whatsapp-logo"></i> Cobrar no WhatsApp
                                                        </a>
                                                    <?php else: ?>
                                                        <span style="font-size: 12px; color: #9CA3AF;">Sem WhatsApp cadastrado</span>
                                                    <?php endif; ?>
                                                </td>
                                            </tr>
                                        <?php endforeach; ?>
                                    <?php endif; ?>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>

                <!-- TAB 3: API MERCADO PAGO -->
                <div class="view-tab" id="tab-mercadopago">
                    <div class="card" style="max-width: 800px;">
                        <div class="card-header">
                            <h2><i class="ph-bold ph-qr-code" style="color: #0D6EFD;"></i> Integração Mercado Pago (Pix Automático)</h2>
                        </div>
                        <div class="card-body">
                            <form method="POST">
                                <input type="hidden" name="action" value="save_settings">
                                <div class="form-group">
                                    <label>Access Token de Produção (Mercado Pago):</label>
                                    <div style="position: relative;">
                                        <input type="password" id="txtMpToken" name="mp_access_token" class="form-control" value="<?= htmlspecialchars($currentMpToken) ?>" required style="padding-right: 80px; font-family: monospace;">
                                        <button type="button" onclick="togglePasswordVisibility('txtMpToken', this)" style="position: absolute; right: 10px; top: 50%; transform: translateY(-50%); background: none; border: none; font-size: 12px; color: #6C757D; cursor: pointer; font-weight: 600;">MOSTRAR</button>
                                    </div>
                                    <small style="color: var(--text-muted); font-size: 11px; margin-top: 4px; display: block;">
                                        Obtenha em: <em>Mercado Pago Developers -> Suas Integrações -> Credenciais de Produção</em>.
                                    </small>
                                </div>

                                <div style="display: flex; gap: 10px; margin-bottom: 25px;">
                                    <button type="button" class="btn-secondary" id="btnTestMp" onclick="testMercadoPagoAjax()" style="display: flex; align-items: center; gap: 6px;">
                                        <i class="ph-bold ph-plugs-connected"></i>
                                        <span>Testar Conexão com Mercado Pago</span>
                                    </button>
                                    <button type="submit" class="btn-submit">Salvar Credencial</button>
                                </div>

                                <div id="boxMpTestResult" style="display: none; margin-bottom: 20px;"></div>
                            </form>

                            <hr style="border: 0; border-top: 1px solid var(--border-color); margin: 25px 0;">

                            <h3 style="font-size: 14px; font-weight: 700; margin-bottom: 8px;">URL do Webhook Instantâneo (Notificações Pix)</h3>
                            <p style="font-size: 12px; color: var(--text-muted); margin-bottom: 12px;">Cadastre esta URL nas Notificações do Mercado Pago para liberação ultra-rápida após o Pix ser pago:</p>
                            <div style="display: flex; gap: 8px;">
                                <input type="text" readonly class="form-control" value="<?= htmlspecialchars($webhookUrl) ?>" style="background: #F9FAFB; font-family: monospace; font-size: 12px;">
                                <button type="button" class="btn-secondary" onclick="copyText('<?= htmlspecialchars($webhookUrl) ?>', 'URL do Webhook')" style="white-space: nowrap;">
                                    <i class="ph-bold ph-copy"></i> Copiar URL
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- TAB 4: HISTÓRICO PIX -->
                <div class="view-tab" id="tab-pix-history">
                    <div class="card">
                        <div class="card-header">
                            <h2><i class="ph-bold ph-receipt"></i> Histórico de Pagamentos Pix</h2>
                            <span style="font-size: 12px; color: var(--text-muted);">Últimos 50 pagamentos</span>
                        </div>
                        <div class="table-responsive">
                            <table>
                                <thead>
                                    <tr>
                                        <th>Data</th>
                                        <th>Sistema</th>
                                        <th>Cliente</th>
                                        <th>Chave Serial</th>
                                        <th>Valor</th>
                                        <th>ID Mercado Pago</th>
                                        <th>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <?php if (empty($pagamentos)): ?>
                                        <tr><td colspan="7" style="text-align: center; color: var(--text-muted); padding: 35px;">Nenhum pagamento Pix registrado ainda.</td></tr>
                                    <?php else: ?>
                                        <?php foreach ($pagamentos as $p): ?>
                                            <tr>
                                                <td><?= date('d/m/Y H:i', strtotime($p['data_criacao'])) ?></td>
                                                <td><span class="system-badge"><?= htmlspecialchars($p['sistema'] ?? 'MasterServicePro') ?></span></td>
                                                <td><strong><?= htmlspecialchars($p['cliente_nome']) ?></strong></td>
                                                <td><span class="serial-tag"><?= htmlspecialchars($p['chave_licenca']) ?></span></td>
                                                <td><strong>R$ <?= number_format($p['valor'], 2, ',', '.') ?></strong></td>
                                                <td style="font-family: monospace; font-size: 12px; color: var(--text-muted);"><?= htmlspecialchars($p['mp_payment_id']) ?></td>
                                                <td>
                                                    <?php if ($p['status'] === 'approved'): ?>
                                                        <span class="badge-status status-active">Aprovado</span>
                                                    <?php else: ?>
                                                        <span class="badge-status status-expired">Pendente</span>
                                                    <?php endif; ?>
                                                </td>
                                            </tr>
                                        <?php endforeach; ?>
                                    <?php endif; ?>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>

                <!-- TAB 5: CONFIGURAÇÕES -->
                <div class="view-tab" id="tab-settings">
                    <div class="card" style="max-width: 600px;">
                        <div class="card-header">
                            <h2><i class="ph-bold ph-gear"></i> Configurações Gerais do Painel</h2>
                        </div>
                        <div class="card-body">
                            <form method="POST">
                                <input type="hidden" name="action" value="save_settings">
                                <div class="form-group">
                                    <label>Nome do Administrador:</label>
                                    <input type="text" name="admin_name" class="form-control" value="<?= htmlspecialchars($adminUser) ?>" required>
                                </div>
                                <div class="form-group">
                                    <label>Alterar Senha do Painel:</label>
                                    <input type="password" name="admin_password" class="form-control" placeholder="Deixe em branco para manter a senha atual">
                                </div>
                                <button type="submit" class="btn-submit">Salvar Alterações</button>
                            </form>
                        </div>
                    </div>
                </div>

            </div>
        </main>
    </div>

    <!-- MODAL: NOVA LICENÇA -->
    <div class="modal" id="modalNewLicense">
        <div class="modal-card">
            <div class="modal-header">
                <h3><i class="ph-bold ph-plus-circle" style="color: #0D6EFD;"></i> Cadastrar Nova Licença</h3>
                <button type="button" class="btn-close-modal" onclick="closeNewLicenseModal()">&times;</button>
            </div>
            <form method="POST">
                <input type="hidden" name="action" value="create_license">
                <div class="modal-body">
                    <div class="form-group">
                        <label>Sistema / Software:</label>
                        <select name="sistema" class="form-control">
                            <option value="MasterServicePro" selected>MasterServicePro</option>
                            <option value="MasterUnlocker">MasterUnlocker</option>
                            <option value="Outro Sistema">Outro Sistema</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label>Nome do Cliente / Razão Social:</label>
                        <input type="text" name="cliente_nome" class="form-control" placeholder="Ex: Assistência Técnica Central" required>
                    </div>
                    <div class="form-row">
                        <div class="form-group">
                            <label>CPF ou CNPJ:</label>
                            <input type="text" name="cliente_cpf_cnpj" class="form-control" placeholder="000.000.000-00">
                        </div>
                        <div class="form-group">
                            <label>WhatsApp / Fone:</label>
                            <input type="text" name="cliente_telefone" class="form-control" placeholder="(11) 99999-9999">
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group">
                            <label>Validade Inicial (Dias):</label>
                            <input type="number" name="dias_validade" class="form-control" value="30" min="1" required>
                        </div>
                        <div class="form-group">
                            <label>Valor Mensalidade (R$):</label>
                            <input type="number" step="0.01" name="valor_mensalidade" class="form-control" value="<?= DEFAULT_MONTHLY_PRICE ?>" required>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn-secondary" onclick="closeNewLicenseModal()">Cancelar</button>
                    <button type="submit" class="btn-submit">+ Gerar Chave de Licença</button>
                </div>
            </form>
        </div>
    </div>

    <!-- Scripts -->
    <script>
        // All JavaScript comments must use ASCII characters only

        function switchTab(tabId, el) {
            document.querySelectorAll('.view-tab').forEach(t => t.classList.remove('active'));
            document.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));

            const targetTab = document.getElementById(tabId);
            if (targetTab) targetTab.classList.add('active');
            if (el) el.classList.add('active');

            // Update title
            const titles = {
                'tab-dashboard': 'Dashboard',
                'tab-licencas': 'Todas as Licenças',
                'tab-expiring': 'Vencendo em Breve',
                'tab-mercadopago': 'Integração Mercado Pago',
                'tab-pix-history': 'Histórico Pix',
                'tab-settings': 'Configurações'
            };
            document.getElementById('pageTitle').innerText = titles[tabId] || 'Dashboard';
            document.getElementById('breadcrumbCurrent').innerText = titles[tabId] || 'Dashboard';
        }

        function openNewLicenseModal() {
            document.getElementById('modalNewLicense').classList.add('active');
        }

        function closeNewLicenseModal() {
            document.getElementById('modalNewLicense').classList.remove('active');
        }

        // Live Real-Time Filter for Licenses Table
        function filterLicensesTable() {
            const query = document.getElementById('txtSearchLicenses').value.toLowerCase().trim();
            const filterSys = document.getElementById('selFilterSystem').value.toLowerCase();
            const filterSt = document.getElementById('selFilterStatus').value.toLowerCase();

            const rows = document.querySelectorAll('#tblLicenses tbody tr.license-row');
            let visibleCount = 0;

            rows.forEach(row => {
                const searchData = row.getAttribute('data-search') || '';
                const sysData = (row.getAttribute('data-system') || '').toLowerCase();
                const stData = (row.getAttribute('data-status') || '').toLowerCase();

                const matchQuery = !query || searchData.includes(query);
                const matchSys = !filterSys || sysData === filterSys;
                const matchSt = !filterSt || stData === filterSt;

                if (matchQuery && matchSys && matchSt) {
                    row.style.display = '';
                    visibleCount++;
                } else {
                    row.style.display = 'none';
                }
            });

            document.getElementById('lblLicensesCount').innerText = `Exibindo ${visibleCount} licença(s)`;
        }

        // Copy Text to Clipboard Helper
        function copyText(text, label) {
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(text).then(() => {
                    alert(`${label} copiado para a área de transferência!`);
                });
            } else {
                const tmp = document.createElement('textarea');
                tmp.value = text;
                document.body.appendChild(tmp);
                tmp.select();
                document.execCommand('copy');
                document.body.removeChild(tmp);
                alert(`${label} copiado!`);
            }
        }

        // Toggle password visibility
        function togglePasswordVisibility(inputId, btn) {
            const input = document.getElementById(inputId);
            if (input.type === 'password') {
                input.type = 'text';
                btn.innerText = 'OCULTAR';
            } else {
                input.type = 'password';
                btn.innerText = 'MOSTRAR';
            }
        }

        // AJAX test Mercado Pago connection
        function testMercadoPagoAjax() {
            const token = document.getElementById('txtMpToken').value.trim();
            const btn = document.getElementById('btnTestMp');
            const resultBox = document.getElementById('boxMpTestResult');

            btn.disabled = true;
            btn.querySelector('span').innerText = 'Testando comunicação...';
            resultBox.style.display = 'none';

            const formData = new FormData();
            formData.append('token', token);

            fetch('index.php?ajax=test_mp', {
                method: 'POST',
                body: formData
            })
            .then(res => res.json())
            .then(data => {
                btn.disabled = false;
                btn.querySelector('span').innerText = 'Testar Conexão com Mercado Pago';
                resultBox.style.display = 'block';

                if (data.success) {
                    resultBox.className = 'alert-banner alert-success';
                    resultBox.innerHTML = `<i class="ph-bold ph-check-circle" style="font-size:18px;"></i> <div><strong>Sucesso!</strong> ${data.message} (${data.methods_count || 0} métodos de pagamento disponíveis)</div>`;
                } else {
                    resultBox.className = 'alert-banner alert-danger';
                    resultBox.innerHTML = `<i class="ph-bold ph-warning-circle" style="font-size:18px;"></i> <div><strong>Atenção:</strong> ${data.message}</div>`;
                }
            })
            .catch(err => {
                btn.disabled = false;
                btn.querySelector('span').innerText = 'Testar Conexão com Mercado Pago';
                resultBox.style.display = 'block';
                resultBox.className = 'alert-banner alert-danger';
                resultBox.innerHTML = `<i class="ph-bold ph-warning-circle" style="font-size:18px;"></i> <div>Erro ao conectar ao servidor local: ${err.message}</div>`;
            });
        }
    </script>
<?php endif; ?>

</body>
</html>
