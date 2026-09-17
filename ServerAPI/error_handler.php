<?php
// Global Error Handler for MasterDevSolutions Licensing System
// All comments must use ASCII characters only.

function isApiOrAjaxRequest() {
    // Check if URL points to API endpoint
    $uri = $_SERVER['REQUEST_URI'] ?? '';
    if (strpos($uri, '/api/') !== false) {
        return true;
    }

    // Check if client expects JSON
    $accept = $_SERVER['HTTP_ACCEPT'] ?? '';
    if (strpos($accept, 'application/json') !== false) {
        return true;
    }

    // Check AJAX XMLHttpRequest header
    $requestedWith = $_SERVER['HTTP_X_REQUESTED_WITH'] ?? '';
    if (strtolower($requestedWith) === 'xmlhttprequest') {
        return true;
    }

    return false;
}

function showGlobalError($type, $title, $description, $technicalDetails = '', $httpCode = 500) {
    http_response_code($httpCode);

    if (isApiOrAjaxRequest()) {
        header('Content-Type: application/json; charset=utf-8');
        header('Access-Control-Allow-Origin: *');
        echo json_encode([
            'success' => false,
            'error_type' => $type,
            'title' => $title,
            'message' => $description,
            'details' => $technicalDetails
        ], JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
        exit;
    }

    // Otherwise, render full MasterDevSolutions HTML error page
    header('Content-Type: text/html; charset=utf-8');

    // Select color accent and icon SVG based on error type
    $accentColor = '#EF4444'; // Red default
    $iconSvg = '';
    $hintsHtml = '';

    if ($type === 'db_connection') {
        $accentColor = '#F59E0B'; // Amber / Orange
        $iconSvg = '<svg width="56" height="56" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><ellipse cx="12" cy="5" rx="9" ry="3"></ellipse><path d="M21 12c0 1.66-4 3-9 3s-9-1.34-9-3"></path><path d="M3 5v14c0 1.66 4 3 9 3s9-1.34 9-3V5"></path><line x1="3" y1="12" x2="21" y2="12"></line><line x1="1" y1="1" x2="23" y2="23" stroke="#EF4444" stroke-width="2.5"></line></svg>';
        $hintsHtml = '
            <div class="hints-box">
                <h4><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg> Como resolver este problema na Locaweb / Hospedagem:</h4>
                <ul>
                    <li>Abra o arquivo <code>config.php</code> na raiz da sua pasta <code>server_api/</code>.</li>
                    <li>Verifique se o <strong>DB_HOST</strong> está correto (na Locaweb geralmente é <code>mysql.seudominio.com.br</code> ou um IP interno, e não <code>localhost</code>).</li>
                    <li>Confirme se o <strong>DB_NAME</strong>, <strong>DB_USER</strong> e <strong>DB_PASS</strong> correspondem exatamente ao banco criado no Painel da Locaweb.</li>
                    <li>Verifique se o banco de dados foi criado e o arquivo <code>database.sql</code> foi importado no phpMyAdmin.</li>
                </ul>
            </div>
        ';
    } else if ($type === 'file_not_found') {
        $accentColor = '#3B82F6'; // Blue
        $iconSvg = '<svg width="56" height="56" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><circle cx="11" cy="14" r="3"></circle><line x1="13.5" y1="16.5" x2="17" y2="20"></line></svg>';
        $hintsHtml = '
            <div class="hints-box">
                <h4><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg> O que verificar:</h4>
                <ul>
                    <li>Confira se a URL digitada no navegador está correta.</li>
                    <li>Verifique se todos os arquivos foram enviados corretamente via FileZilla para o diretório de destino.</li>
                </ul>
            </div>
        ';
    } else if ($type === 'file_load_failed') {
        $accentColor = '#EC4899'; // Pink / Purple
        $iconSvg = '<svg width="56" height="56" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="12" y1="18" x2="12" y2="12"></line><line x1="12" y1="9" x2="12.01" y2="9"></line></svg>';
        $hintsHtml = '
            <div class="hints-box">
                <h4><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg> O que verificar:</h4>
                <ul>
                    <li>Verifique as permissões de leitura dos arquivos no servidor (recomendado 644 para arquivos e 755 para pastas).</li>
                    <li>Confirme se o arquivo não foi corrompido durante o upload no FileZilla.</li>
                </ul>
            </div>
        ';
    } else {
        $iconSvg = '<svg width="56" height="56" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="8" x2="12" y2="12"></line><line x1="12" y1="16" x2="12.01" y2="16"></line></svg>';
    }
?>
<!DOCTYPE html>
<html lang="pt-BR">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title><?= htmlspecialchars($title) ?> - MasterDevSolutions</title>
    <style>
        :root {
            --bg-body: #0A0D14;
            --bg-card: #151822;
            --bg-subcard: #1C202D;
            --text-main: #FFFFFF;
            --text-secondary: #94A3B8;
            --text-muted: #64748B;
            --border-color: #262B3B;
            --accent: <?= $accentColor ?>;
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
        }

        body {
            background-color: var(--bg-body);
            color: var(--text-main);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .error-card {
            background: var(--bg-card);
            border: 1px solid var(--border-color);
            border-radius: 16px;
            max-width: 620px;
            width: 100%;
            padding: 35px;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.7);
            position: relative;
            overflow: hidden;
        }

        .error-card::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 4px;
            background: var(--accent);
        }

        .header-brand {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 25px;
            padding-bottom: 16px;
            border-bottom: 1px solid var(--border-color);
        }

        .brand-name {
            font-size: 15px;
            font-weight: 700;
            letter-spacing: -0.3px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .brand-name span {
            color: #38BDF8;
        }

        .status-badge {
            background: rgba(255, 255, 255, 0.05);
            border: 1px solid var(--border-color);
            padding: 4px 10px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 600;
            color: var(--text-secondary);
        }

        .icon-wrapper {
            width: 76px;
            height: 76px;
            border-radius: 50%;
            background: rgba(255, 255, 255, 0.03);
            border: 1px solid var(--border-color);
            display: flex;
            align-items: center;
            justify-content: center;
            color: var(--accent);
            margin-bottom: 20px;
        }

        h2.error-title {
            font-size: 22px;
            font-weight: 700;
            margin-bottom: 10px;
            color: var(--text-main);
        }

        p.error-desc {
            font-size: 14px;
            color: var(--text-secondary);
            line-height: 1.6;
            margin-bottom: 20px;
        }

        .hints-box {
            background: var(--bg-subcard);
            border: 1px solid var(--border-color);
            border-radius: 10px;
            padding: 16px 20px;
            margin-bottom: 22px;
            text-align: left;
        }

        .hints-box h4 {
            font-size: 13px;
            color: #FCD34D;
            margin-bottom: 10px;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .hints-box ul {
            padding-left: 20px;
            font-size: 12px;
            color: var(--text-secondary);
            line-height: 1.8;
        }

        .hints-box code {
            background: rgba(0, 0, 0, 0.3);
            padding: 2px 6px;
            border-radius: 4px;
            color: #60A5FA;
            font-family: monospace;
            font-size: 12px;
        }

        .details-toggle {
            background: transparent;
            border: none;
            color: var(--text-muted);
            font-size: 12px;
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 6px;
            margin-bottom: 15px;
            padding: 0;
            text-decoration: underline;
        }

        .details-toggle:hover {
            color: var(--text-main);
        }

        .technical-details {
            display: none;
            background: #090B10;
            border: 1px solid var(--border-color);
            border-radius: 8px;
            padding: 12px 14px;
            font-family: monospace;
            font-size: 11px;
            color: #F87171;
            word-break: break-all;
            margin-bottom: 20px;
            max-height: 150px;
            overflow-y: auto;
            text-align: left;
        }

        .actions-row {
            display: flex;
            align-items: center;
            justify-content: flex-end;
            gap: 12px;
            margin-top: 10px;
        }

        .btn-retry {
            background: #0D6EFD;
            color: #FFFFFF;
            border: none;
            padding: 10px 20px;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 600;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            transition: 0.2s;
        }

        .btn-retry:hover {
            background: #0B5ED7;
        }

        .btn-back {
            background: var(--bg-subcard);
            color: var(--text-secondary);
            border: 1px solid var(--border-color);
            padding: 9px 18px;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 500;
            cursor: pointer;
            text-decoration: none;
            transition: 0.2s;
        }

        .btn-back:hover {
            color: var(--text-main);
            border-color: #4B5563;
        }

        .footer-note {
            margin-top: 25px;
            font-size: 11px;
            color: var(--text-muted);
            text-align: center;
            border-top: 1px solid rgba(255, 255, 255, 0.05);
            padding-top: 15px;
        }
    </style>
</head>
<body>

    <div class="error-card">
        <div class="header-brand">
            <div class="brand-name">
                MasterDev<span>Solutions</span>
            </div>
            <div class="status-badge">
                HTTP <?= (int)$httpCode ?>
            </div>
        </div>

        <div class="icon-wrapper">
            <?= $iconSvg ?>
        </div>

        <h2 class="error-title"><?= htmlspecialchars($title) ?></h2>
        <p class="error-desc"><?= htmlspecialchars($description) ?></p>

        <?= $hintsHtml ?>

        <?php if (!empty($technicalDetails)): ?>
            <button class="details-toggle" type="button" onclick="toggleDetails()">
                <span>Exibir detalhes técnicos do erro</span>
            </button>
            <div class="technical-details" id="boxTechDetails">
                <?= htmlspecialchars($technicalDetails) ?>
            </div>
        <?php endif; ?>

        <div class="actions-row">
            <a href="javascript:history.back()" class="btn-back">Voltar</a>
            <button class="btn-retry" type="button" onclick="window.location.reload()">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="23 4 23 10 17 10"></polyline><polyline points="1 20 1 14 7 14"></polyline><path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15"></path></svg>
                Tentar Novamente
            </button>
        </div>

        <div class="footer-note">
            MasterDevSolutions Licensing System &bull; Sistema de Diagnóstico
        </div>
    </div>

    <script>
        function toggleDetails() {
            const box = document.getElementById('boxTechDetails');
            if (box) {
                box.style.display = (box.style.display === 'block') ? 'none' : 'block';
            }
        }
    </script>
</body>
</html>
<?php
    exit;
}

// Global Exception Handler
set_exception_handler(function($exception) {
    showGlobalError(
        'exception',
        'Ocorreu uma Exceção no Sistema',
        $exception->getMessage(),
        $exception->getFile() . ':' . $exception->getLine() . "\n" . $exception->getTraceAsString(),
        500
    );
});

// Global Safe File Require Helper
function safeRequireFile($filePath, $friendlyName = '') {
    if (!file_exists($filePath)) {
        showGlobalError(
            'file_not_found',
            'Arquivo Não Encontrado',
            'O arquivo necessário ' . ($friendlyName ? "($friendlyName)" : '') . ' não foi encontrado no servidor.',
            'Caminho: ' . $filePath,
            404
        );
    }

    if (!is_readable($filePath)) {
        showGlobalError(
            'file_load_failed',
            'Falha ao Carregar Arquivo',
            'O arquivo existe mas não possui permissão de leitura no servidor.',
            'Caminho: ' . $filePath,
            403
        );
    }

    return require_once $filePath;
}
