-- Database schema for MasterDevSolutions Licensing and Management Dashboard
-- Compatible with MySQL 5.7+ / MariaDB 10.2+
-- All comments must use ASCII characters only.

CREATE TABLE IF NOT EXISTS `licencas` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `sistema` VARCHAR(100) NOT NULL DEFAULT 'MasterServicePro',
  `chave_licenca` VARCHAR(50) NOT NULL UNIQUE,
  `cliente_nome` VARCHAR(150) NOT NULL,
  `cliente_cpf_cnpj` VARCHAR(20) NULL,
  `cliente_telefone` VARCHAR(20) NULL,
  `hwid` VARCHAR(128) NULL,
  `data_vencimento` DATETIME NOT NULL,
  `status` ENUM('ativa', 'vencida', 'bloqueada') NOT NULL DEFAULT 'ativa',
  `valor_mensalidade` DECIMAL(10,2) NOT NULL DEFAULT 80.00,
  `observacoes` TEXT NULL,
  `data_criacao` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_ultimo_pagamento` DATETIME NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `pagamentos_pix` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `id_licenca` INT NOT NULL,
  `mp_payment_id` BIGINT NOT NULL UNIQUE,
  `chave_licenca` VARCHAR(50) NOT NULL,
  `qr_code` TEXT NOT NULL,
  `qr_code_base64` LONGTEXT NOT NULL,
  `copia_cola` TEXT NOT NULL,
  `valor` DECIMAL(10,2) NOT NULL,
  `status` VARCHAR(30) NOT NULL DEFAULT 'pending',
  `data_criacao` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `data_aprovacao` DATETIME NULL,
  CONSTRAINT `fk_pix_licenca` FOREIGN KEY (`id_licenca`) REFERENCES `licencas` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Table to store system settings and API credentials dynamically from panel
CREATE TABLE IF NOT EXISTS `configuracoes` (
  `chave` VARCHAR(50) NOT NULL PRIMARY KEY,
  `valor` TEXT NULL,
  `data_atualizacao` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Seed initial settings
INSERT IGNORE INTO `configuracoes` (`chave`, `valor`) VALUES
('panel_title', 'MasterDevSolutions'),
('admin_name', 'niloneto'),
('admin_password', 'admin123'),
('mp_access_token', 'APP_USR-SEU-ACCESS-TOKEN-AQUI'),
('default_monthly_price', '80.00');

-- Seed initial test license key for MasterServicePro (valid for 30 days)
INSERT IGNORE INTO `licencas` 
(`sistema`, `chave_licenca`, `cliente_nome`, `cliente_cpf_cnpj`, `cliente_telefone`, `data_vencimento`, `status`, `valor_mensalidade`)
VALUES 
('MasterServicePro', 'MSP-TEST-1234-ABCD', 'Cliente Teste Demonstracao', '000.000.000-00', '(11) 99999-9999', DATE_ADD(NOW(), INTERVAL 30 DAY), 'ativa', 80.00);
