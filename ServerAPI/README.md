# Painel de Gestao de Licencas e Pix - MasterDevSolutions (PHP / MySQL)

Este modulo contem a central administrativa e a API de controle de licencas multi-sistemas (iniciando por MasterServicePro) e intermediacao de pagamentos Pix via Mercado Pago da **MasterDevSolutions**.

## Arquivos para Subir na Hospedagem (Locaweb / cPanel / Hostinger / etc.)

Todos os arquivos desta pasta `ServerAPI/` devem ser enviados para a sua hospedagem (por exemplo, na pasta `public_html/licenca/` ou em um subdominio como `licenca.masterdevsolutions.com.br`):

- `config.php` - Configuracoes de conexao MySQL e integracao com Mercado Pago
- `database.sql` - Script SQL com tabelas `licencas`, `pagamentos_pix` e `configuracoes`
- `admin/index.php` - Painel de Gestao MasterDevSolutions (Dashboard, Licencas, API MP, WhatsApp)
- `api/check.php` - Endpoint de consulta, ativacao e verificacao de HWID
- `api/pix_generate.php` - Endpoint de geracao de QR Code Pix e Copia e Cola
- `api/pix_status.php` - Endpoint de checagem e renovacao automatica (+30 dias)
- `api/webhook.php` - Webhook para notificacoes instantaneas do Mercado Pago

## Passo a Passo para Instalacao na Hospedagem (ex: Locaweb):

1. Acesse o Painel de Controle da sua hospedagem e va em **Bancos de Dados MySQL**.
2. Crie um novo banco de dados (ex: `licencas_masterdev`) e anote o host do banco, usuario e senha.
3. Abra o **phpMyAdmin**, selecione o banco criado e va na aba **Importar**. Selecione o arquivo `database.sql` e execute.
4. Abra o arquivo `config.php` e preencha:
   - `DB_HOST`, `DB_NAME`, `DB_USER`, `DB_PASS` com os dados do banco criado na Locaweb.
5. Acesse seu painel em: `https://seusite.com.br/licenca/admin/` (ou no subdominio configurado).
   - Senha padrao inicial: `admin123`
6. No painel, va na aba **API Mercado Pago**, insira seu Access Token de Producao e clique em **Testar Conexao com Mercado Pago** para validar a integracao em tempo real!
