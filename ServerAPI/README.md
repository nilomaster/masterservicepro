# Servidor Central de Licenciamento - MasterServicePro (PHP / MySQL)

Este modulo contem a API de controle de licencas e intermediacao de pagamentos Pix via Mercado Pago para o MasterServicePro.

## Arquivos para Subir na Hospedagem (cPanel / Hostinger / etc.)

Todos os arquivos desta pasta `ServerAPI/` devem ser enviados para a sua hospedagem (por exemplo, na pasta `public_html/licenca/` ou em um subdominio `licenca.seudominio.com.br`):

- `config.php` - Configuracoes de banco de dados e Access Token do Mercado Pago
- `database.sql` - Script SQL para criar as tabelas no phpMyAdmin
- `api/check.php` - Endpoint de consulta e ativacao de licencas
- `api/pix_generate.php` - Endpoint de geracao de QR Code Pix
- `api/pix_status.php` - Endpoint de checagem e renovacao automatica (+30 dias)
- `api/webhook.php` - Webhook para notificacoes instantaneas do Mercado Pago
- `admin/index.php` - Painel Web de gerenciamento de licencas e pagamentos

## Passo a Passo para Instalacao na Hospedagem:

1. Acesse o cPanel da sua hospedagem e va em **Bancos de Dados MySQL**.
2. Crie um novo banco de dados (ex: `masterservicepro_licencas`) e um usuario com permissao total.
3. Abra o **phpMyAdmin**, selecione o banco criado e va na aba **Importar**. Selecione o arquivo `database.sql` e execute.
4. Abra o arquivo `config.php` e preencha:
   - `DB_HOST`, `DB_NAME`, `DB_USER`, `DB_PASS` com os dados do banco criado.
   - `MP_ACCESS_TOKEN` com o seu Access Token de Producao do Mercado Pago (obtido em *Mercado Pago Developers -> Suas Integracoes -> Credenciais de Producao*).
   - `ADMIN_PASSWORD` com a senha que voce deseja usar para acessar o painel de administracao.
5. Acesse seu painel em: `https://seusite.com.br/licenca/admin/` e crie as licencas para seus clientes!
