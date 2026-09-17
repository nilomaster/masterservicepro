<?php
// Root index file to redirect visitors to the admin dashboard
// All comments must use ASCII characters only.

if (!file_exists(__DIR__ . '/admin/index.php')) {
    require_once __DIR__ . '/error_handler.php';
    showGlobalError(
        'file_not_found',
        'Painel Administrativo Não Encontrado',
        'A pasta admin/ ou o arquivo index.php não foram localizados no servidor.',
        'Verifique se a pasta admin foi enviada via FTP/FileZilla.',
        404
    );
}

header('Location: admin/');
exit;
