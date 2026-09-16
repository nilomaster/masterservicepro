<?php
// Admin Panel for Managing MasterServicePro Licenses and Pix Payments
// All comments must use ASCII characters only.

session_start();
require_once __DIR__ . '/../config.php';

$pdo = getDbConnection();

// Authentication Handling
if (isset($_POST['action']) && $_POST['action'] === 'login') {
    $password = $_POST['password'] ?? '';
    if ($password === ADMIN_PASSWORD) {
        $_SESSION['msp_admin_logged'] = true;
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

$isLogged = !empty($_SESSION['msp_admin_logged']);

// If logged in, handle admin actions
$msgSuccess = '';
$msgError = '';

if ($isLogged && $_SERVER['REQUEST_METHOD'] === 'POST') {
    $postAction = $_POST['action'] ?? '';

    // Create New License
    if ($postAction === 'create_license') {
        $nome = trim($_POST['cliente_nome'] ?? '');
        $cpfCnpj = trim($_POST['cliente_cpf_cnpj'] ?? '');
        $telefone = trim($_POST['cliente_telefone'] ?? '');
        $dias = (int)($_POST['dias_validade'] ?? 30);
        $valor = (float)($_POST['valor_mensalidade'] ?? DEFAULT_MONTHLY_PRICE);

        if (empty($nome)) {
            $msgError = 'O nome do cliente é obrigatório.';
        } else {
            // Generate unique serial format: MSP-XXXX-XXXX
            $randomPart1 = strtoupper(substr(bin2hex(random_bytes(2)), 0, 4));
            $randomPart2 = strtoupper(substr(bin2hex(random_bytes(2)), 0, 4));
            $serial = 'MSP-' . $randomPart1 . '-' . $randomPart2;

            $vencimento = (new DateTime())->modify('+' . $dias . ' days')->format('Y-m-d H:i:s');

            $stmt = $pdo->prepare("
                INSERT INTO licencas 
                (chave_licenca, cliente_nome, cliente_cpf_cnpj, cliente_telefone, data_vencimento, status, valor_mensalidade, data_criacao)
                VALUES 
                (:chave, :nome, :cpf, :tel, :vencimento, 'ativa', :valor, NOW())
            ");
            $stmt->execute([
                'chave' => $serial,
                'nome' => $nome,
                'cpf' => $cpfCnpj,
                'tel' => $telefone,
                'vencimento' => $vencimento,
                'valor' => $valor
            ]);

            $msgSuccess = 'Nova licença criada com sucesso: ' . $serial;
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
}

// Fetch dashboard data if logged in
$licencas = [];
$pagamentos = [];
$totalLicencas = 0;
$totalAtivas = 0;
$totalVencidas = 0;
$totalRecebido = 0.0;

if ($isLogged) {
    $licencas = $pdo->query('SELECT * FROM licencas ORDER BY id DESC')->fetchAll();
    $pagamentos = $pdo->query('SELECT p.*, l.cliente_nome FROM pagamentos_pix p JOIN licencas l ON p.id_licenca = l.id ORDER BY p.id DESC LIMIT 50')->fetchAll();

    $totalLicencas = count($licencas);
    foreach ($licencas as $l) {
        $now = new DateTime();
        $venc = new DateTime($l['data_vencimento']);
        if ($l['status'] === 'ativa' && $now <= $venc) {
            $totalAtivas++;
        } else {
            $totalVencidas++;
        }
    }

    $sumStmt = $pdo->query("SELECT SUM(valor) as total FROM pagamentos_pix WHERE status = 'approved'");
    $sumRow = $sumStmt->fetch();
    $totalRecebido = (float)($sumRow['total'] ?? 0.0);
}
?>
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Painel de Licenças - MasterServicePro</title>
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --bg-body: #0A0D14;
            --bg-card: #151822;
            --bg-card-hover: #1c202d;
            --primary: #4361EE;
            --primary-hover: #3651d4;
            --success: #10B981;
            --warning: #F59E0B;
            --danger: #EF4444;
            --text-main: #FFFFFF;
            --text-muted: #94A3B8;
            --border-color: #262B3B;
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Inter', sans-serif;
        }

        body {
            background-color: var(--bg-body);
            color: var(--text-main);
            min-height: 100vh;
            padding: 20px;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
        }

        .header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding-bottom: 20px;
            border-bottom: 1px solid var(--border-color);
            margin-bottom: 25px;
        }

        .header h1 {
            font-size: 22px;
            font-weight: 700;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .header h1 span {
            color: var(--primary);
        }

        .btn-logout {
            background: #262B3B;
            color: #fff;
            padding: 8px 16px;
            border-radius: 8px;
            text-decoration: none;
            font-size: 13px;
        }

        .btn-logout:hover { background: #32384c; }

        /* Login Card */
        .login-wrapper {
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 80vh;
        }

        .login-card {
            background: var(--bg-card);
            border: 1px solid var(--border-color);
            border-radius: 12px;
            padding: 35px;
            width: 100%;
            max-width: 380px;
            text-align: center;
            box-shadow: 0 10px 30px rgba(0,0,0,0.5);
        }

        .login-card h2 {
            font-size: 20px;
            margin-bottom: 8px;
        }

        .login-card p {
            color: var(--text-muted);
            font-size: 13px;
            margin-bottom: 25px;
        }

        .form-input {
            width: 100%;
            padding: 12px 14px;
            background: #0D1017;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            color: #fff;
            font-size: 14px;
            margin-bottom: 15px;
            outline: none;
        }

        .form-input:focus {
            border-color: var(--primary);
        }

        .btn-primary {
            width: 100%;
            padding: 12px;
            background: var(--primary);
            color: #fff;
            border: none;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: 0.2s;
        }

        .btn-primary:hover {
            background: var(--primary-hover);
        }

        /* Dashboard Stats */
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
            gap: 15px;
            margin-bottom: 25px;
        }

        .stat-card {
            background: var(--bg-card);
            border: 1px solid var(--border-color);
            border-radius: 12px;
            padding: 20px;
        }

        .stat-card .label {
            font-size: 13px;
            color: var(--text-muted);
            margin-bottom: 6px;
        }

        .stat-card .value {
            font-size: 26px;
            font-weight: 700;
        }

        .stat-card.ativas .value { color: var(--success); }
        .stat-card.vencidas .value { color: var(--danger); }
        .stat-card.receita .value { color: #38BDF8; }

        /* Actions & Table */
        .card {
            background: var(--bg-card);
            border: 1px solid var(--border-color);
            border-radius: 12px;
            padding: 22px;
            margin-bottom: 25px;
        }

        .card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 18px;
        }

        .card-header h2 {
            font-size: 16px;
            font-weight: 600;
        }

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
            background: #0D1017;
            color: var(--text-muted);
            padding: 12px;
            font-weight: 600;
            border-bottom: 1px solid var(--border-color);
        }

        td {
            padding: 12px;
            border-bottom: 1px solid rgba(255,255,255,0.04);
            vertical-align: middle;
        }

        tr:hover td {
            background: var(--bg-card-hover);
        }

        .serial-badge {
            background: #1C2333;
            color: #60A5FA;
            padding: 4px 8px;
            border-radius: 6px;
            font-family: monospace;
            font-size: 13px;
            font-weight: 600;
            border: 1px solid rgba(96, 165, 250, 0.2);
        }

        .badge {
            padding: 4px 10px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 600;
            display: inline-block;
        }

        .badge-active { background: rgba(16, 185, 129, 0.15); color: #34D399; }
        .badge-expired { background: rgba(239, 68, 68, 0.15); color: #F87171; }
        .badge-blocked { background: rgba(156, 163, 175, 0.2); color: #9CA3AF; }

        .btn-action {
            padding: 5px 10px;
            border-radius: 6px;
            border: none;
            font-size: 11px;
            font-weight: 600;
            cursor: pointer;
            margin-right: 4px;
            transition: 0.2s;
        }

        .btn-extend { background: #1E3A8A; color: #93C5FD; }
        .btn-extend:hover { background: #1D4ED8; color: #fff; }
        .btn-toggle { background: #374151; color: #E5E7EB; }
        .btn-toggle:hover { background: #4B5563; }
        .btn-reset-hwid { background: #4A1D24; color: #FCA5A5; }
        .btn-reset-hwid:hover { background: #7F1D1D; color: #fff; }

        /* Form inline */
        .form-grid {
            display: grid;
            grid-template-columns: 2fr 1fr 1fr 1fr 1fr auto;
            gap: 10px;
            align-items: center;
        }

        .alert {
            padding: 12px 16px;
            border-radius: 8px;
            font-size: 13px;
            margin-bottom: 20px;
        }

        .alert-success { background: rgba(16, 185, 129, 0.15); color: #34D399; border: 1px solid rgba(16, 185, 129, 0.3); }
        .alert-error { background: rgba(239, 68, 68, 0.15); color: #F87171; border: 1px solid rgba(239, 68, 68, 0.3); }
    </style>
</head>
<body>

<div class="container">
    <?php if (!$isLogged): ?>
        <div class="login-wrapper">
            <div class="login-card">
                <h2>MasterService<span>Pro</span></h2>
                <p>Painel de Controle de Licenças e Mensalidades</p>
                <?php if (!empty($loginError)): ?>
                    <div class="alert alert-error"><?= htmlspecialchars($loginError) ?></div>
                <?php endif; ?>
                <form method="POST">
                    <input type="hidden" name="action" value="login">
                    <input type="password" name="password" class="form-input" placeholder="Digite a senha de administrador" required autofocus>
                    <button type="submit" class="btn-primary">Entrar no Painel</button>
                </form>
            </div>
        </div>
    <?php else: ?>
        <div class="header">
            <h1>MasterService<span>Pro</span> Licenciamento</h1>
            <div>
                <a href="?logout=1" class="btn-logout">Sair</a>
            </div>
        </div>

        <?php if (!empty($msgSuccess)): ?>
            <div class="alert alert-success"><?= htmlspecialchars($msgSuccess) ?></div>
        <?php endif; ?>
        <?php if (!empty($msgError)): ?>
            <div class="alert alert-error"><?= htmlspecialchars($msgError) ?></div>
        <?php endif; ?>

        <!-- Stats -->
        <div class="stats-grid">
            <div class="stat-card">
                <div class="label">Total de Licenças</div>
                <div class="value"><?= $totalLicencas ?></div>
            </div>
            <div class="stat-card ativas">
                <div class="label">Licenças Ativas</div>
                <div class="value"><?= $totalAtivas ?></div>
            </div>
            <div class="stat-card vencidas">
                <div class="label">Licenças Vencidas / Bloqueadas</div>
                <div class="value"><?= $totalVencidas ?></div>
            </div>
            <div class="stat-card receita">
                <div class="label">Total Recebido (Pix)</div>
                <div class="value">R$ <?= number_format($totalRecebido, 2, ',', '.') ?></div>
            </div>
        </div>

        <!-- Create License Form -->
        <div class="card">
            <div class="card-header">
                <h2>Gerar Nova Chave de Licença</h2>
            </div>
            <form method="POST">
                <input type="hidden" name="action" value="create_license">
                <div class="form-grid">
                    <input type="text" name="cliente_nome" class="form-input" style="margin-bottom:0;" placeholder="Nome do Cliente / Empresa" required>
                    <input type="text" name="cliente_cpf_cnpj" class="form-input" style="margin-bottom:0;" placeholder="CPF / CNPJ">
                    <input type="text" name="cliente_telefone" class="form-input" style="margin-bottom:0;" placeholder="WhatsApp / Fone">
                    <input type="number" name="dias_validade" class="form-input" style="margin-bottom:0;" value="30" placeholder="Dias">
                    <input type="number" step="0.01" name="valor_mensalidade" class="form-input" style="margin-bottom:0;" value="<?= DEFAULT_MONTHLY_PRICE ?>" placeholder="Valor R$">
                    <button type="submit" class="btn-primary" style="padding: 12px 20px;">+ Gerar Serial</button>
                </div>
            </form>
        </div>

        <!-- Licenses Table -->
        <div class="card">
            <div class="card-header">
                <h2>Licenças Cadastradas</h2>
            </div>
            <div class="table-responsive">
                <table>
                    <thead>
                        <tr>
                            <th>Serial / Chave</th>
                            <th>Cliente</th>
                            <th>Contato</th>
                            <th>HWID (Máquina)</th>
                            <th>Vencimento</th>
                            <th>Status</th>
                            <th>Mensalidade</th>
                            <th>Ações Rápidas</th>
                        </tr>
                    </thead>
                    <tbody>
                        <?php if (empty($licencas)): ?>
                            <tr><td colspan="8" style="text-align: center; color: var(--text-muted); padding: 30px;">Nenhuma licença cadastrada ainda.</td></tr>
                        <?php else: ?>
                            <?php foreach ($licencas as $lic): 
                                $now = new DateTime();
                                $venc = new DateTime($lic['data_vencimento']);
                                $isVencida = ($now > $venc);
                                
                                $badgeClass = 'badge-active';
                                $statusLabel = 'Ativa';
                                if ($lic['status'] === 'bloqueada') {
                                    $badgeClass = 'badge-blocked';
                                    $statusLabel = 'Bloqueada';
                                } else if ($isVencida || $lic['status'] === 'vencida') {
                                    $badgeClass = 'badge-expired';
                                    $statusLabel = 'Vencida';
                                }
                            ?>
                                <tr>
                                    <td><span class="serial-badge"><?= htmlspecialchars($lic['chave_licenca']) ?></span></td>
                                    <td><strong><?= htmlspecialchars($lic['cliente_nome']) ?></strong></td>
                                    <td style="color: var(--text-muted);"><?= htmlspecialchars($lic['cliente_telefone'] ?? '-') ?></td>
                                    <td style="font-size: 11px; color: var(--text-muted);">
                                        <?= !empty($lic['hwid']) ? htmlspecialchars(substr($lic['hwid'], 0, 16)) . '...' : '<span style="color: #64748B;">Livre (não ativada)</span>' ?>
                                    </td>
                                    <td><?= $venc->format('d/m/Y') ?></td>
                                    <td><span class="badge <?= $badgeClass ?>"><?= $statusLabel ?></span></td>
                                    <td>R$ <?= number_format($lic['valor_mensalidade'], 2, ',', '.') ?></td>
                                    <td>
                                        <form method="POST" style="display:inline;">
                                            <input type="hidden" name="action" value="extend_30_days">
                                            <input type="hidden" name="licenca_id" value="<?= $lic['id'] ?>">
                                            <button type="submit" class="btn-action btn-extend" title="Adicionar +30 dias">+30d</button>
                                        </form>
                                        <form method="POST" style="display:inline;">
                                            <input type="hidden" name="action" value="toggle_block">
                                            <input type="hidden" name="licenca_id" value="<?= $lic['id'] ?>">
                                            <button type="submit" class="btn-action btn-toggle"><?= $lic['status'] === 'bloqueada' ? 'Desbloquear' : 'Bloquear' ?></button>
                                        </form>
                                        <?php if (!empty($lic['hwid'])): ?>
                                            <form method="POST" style="display:inline;" onsubmit="return confirm('Resetar HWID permitirá ativar esta licença em outro computador. Confirmar?');">
                                                <input type="hidden" name="action" value="reset_hwid">
                                                <input type="hidden" name="licenca_id" value="<?= $lic['id'] ?>">
                                                <button type="submit" class="btn-action btn-reset-hwid" title="Desvincular do computador atual">Reset HWID</button>
                                            </form>
                                        <?php endif; ?>
                                    </td>
                                </tr>
                            <?php endforeach; ?>
                        <?php endif; ?>
                    </tbody>
                </table>
            </div>
        </div>

        <!-- Recent Pix Payments -->
        <div class="card">
            <div class="card-header">
                <h2>Histórico de Pagamentos Pix</h2>
            </div>
            <div class="table-responsive">
                <table>
                    <thead>
                        <tr>
                            <th>Data</th>
                            <th>Cliente</th>
                            <th>Serial</th>
                            <th>Valor</th>
                            <th>Mercado Pago ID</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        <?php if (empty($pagamentos)): ?>
                            <tr><td colspan="6" style="text-align: center; color: var(--text-muted); padding: 25px;">Nenhum pagamento registrado ainda.</td></tr>
                        <?php else: ?>
                            <?php foreach ($pagamentos as $p): ?>
                                <tr>
                                    <td><?= date('d/m/Y H:i', strtotime($p['data_criacao'])) ?></td>
                                    <td><strong><?= htmlspecialchars($p['cliente_nome']) ?></strong></td>
                                    <td><span class="serial-badge"><?= htmlspecialchars($p['chave_licenca']) ?></span></td>
                                    <td><strong>R$ <?= number_format($p['valor'], 2, ',', '.') ?></strong></td>
                                    <td style="font-family: monospace; font-size: 11px;"><?= htmlspecialchars($p['mp_payment_id']) ?></td>
                                    <td>
                                        <?php if ($p['status'] === 'approved'): ?>
                                            <span class="badge badge-active">Aprovado</span>
                                        <?php else: ?>
                                            <span class="badge badge-expired">Pendente</span>
                                        <?php endif; ?>
                                    </td>
                                </tr>
                            <?php endforeach; ?>
                        <?php endif; ?>
                    </tbody>
                </table>
            </div>
        </div>
    <?php endif; ?>
</div>

</body>
</html>
