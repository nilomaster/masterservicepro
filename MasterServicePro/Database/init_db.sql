-- Script de Inicialização - Master Unlocker PDV / ERP
-- SQL Server 2022 Express

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MasterPDV')
BEGIN
    CREATE DATABASE [MasterPDV];
END
GO

USE [MasterPDV];
GO

-- Tabela de Usuários
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Usuarios](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Nome] NVARCHAR(100) NOT NULL,
        [Login] NVARCHAR(50) NOT NULL UNIQUE,
        [SenhaHash] NVARCHAR(256) NOT NULL,
        [NivelAcesso] NVARCHAR(20) NOT NULL, -- Admin, Gerente, Caixa, Tecnico
        [DataCadastro] DATETIME DEFAULT GETDATE(),
        [DataAtualizacao] DATETIME DEFAULT GETDATE(),
        [Ativo] BIT DEFAULT 1
    );
END

-- Tabela de Clientes
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clientes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Clientes](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Nome] NVARCHAR(150) NOT NULL,
        [CpfCnpj] NVARCHAR(20) NULL,
        [Telefone] NVARCHAR(20) NULL,
        [WhatsApp] NVARCHAR(20) NULL,
        [Email] NVARCHAR(100) NULL,
        [Endereco] NVARCHAR(255) NULL,
        [Historico] NVARCHAR(MAX) NULL,
        [DataCadastro] DATETIME DEFAULT GETDATE(),
        [DataAtualizacao] DATETIME DEFAULT GETDATE(),
        [Ativo] BIT DEFAULT 1
    );
END

-- Tabela de Técnicos
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tecnicos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Tecnicos](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Nome] NVARCHAR(100) NOT NULL,
        [Telefone] NVARCHAR(20) NULL,
        [Especialidade] NVARCHAR(100) NULL,
        [Comissao] DECIMAL(5,2) DEFAULT 0,
        [Status] NVARCHAR(20) DEFAULT 'Disponível',
        [DataCadastro] DATETIME DEFAULT GETDATE(),
        [DataAtualizacao] DATETIME DEFAULT GETDATE(),
        [Ativo] BIT DEFAULT 1
    );
END

-- Tabela de Categorias
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categorias]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Categorias](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Nome] NVARCHAR(100) NOT NULL,
        [DataCadastro] DATETIME DEFAULT GETDATE(),
        [DataAtualizacao] DATETIME DEFAULT GETDATE(),
        [Ativo] BIT DEFAULT 1
    );
END

-- Tabela de Produtos / Peças
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Produtos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Produtos](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [CodigoInterno] NVARCHAR(50) NULL,
        [CodigoBarras] NVARCHAR(100) NULL,
        [Nome] NVARCHAR(150) NOT NULL,
        [IdCategoria] INT FOREIGN KEY REFERENCES [dbo].[Categorias](Id),
        [Marca] NVARCHAR(100) NULL,
        [Modelo] NVARCHAR(100) NULL,
        [Descricao] NVARCHAR(MAX) NULL,
        [PrecoCusto] DECIMAL(18,2) DEFAULT 0,
        [PrecoVenda] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Margem] DECIMAL(5,2) DEFAULT 0,
        [Estoque] INT DEFAULT 0,
        [EstoqueMinimo] INT DEFAULT 0,
        [ImagemUrl] NVARCHAR(255) NULL,
        [Status] NVARCHAR(20) DEFAULT 'Ativo',
        [DataCadastro] DATETIME DEFAULT GETDATE(),
        [DataAtualizacao] DATETIME DEFAULT GETDATE(),
        [Ativo] BIT DEFAULT 1
    );
END

-- Tabela de Movimentação de Estoque
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Estoque]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Estoque](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [IdProduto] INT FOREIGN KEY REFERENCES [dbo].[Produtos](Id),
        [TipoMovimentacao] NVARCHAR(20) NOT NULL, -- ENTRADA/SAIDA
        [Quantidade] INT NOT NULL,
        [DataMovimentacao] DATETIME DEFAULT GETDATE(),
        [Observacao] NVARCHAR(255) NULL
    );
END

-- Tabela de Vendas
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Vendas]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Vendas](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [IdCliente] INT NULL FOREIGN KEY REFERENCES [dbo].[Clientes](Id),
        [Total] DECIMAL(18,2) NOT NULL,
        [Desconto] DECIMAL(18,2) DEFAULT 0,
        [ValorFinal] DECIMAL(18,2) NOT NULL,
        [FormaPagamento] NVARCHAR(50) NOT NULL, -- Dinheiro, Pix, Cartão, Misto
        [Status] NVARCHAR(20) DEFAULT 'Concluída',
        [DataVenda] DATETIME DEFAULT GETDATE()
    );
END

-- Itens da Venda
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItensVenda]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ItensVenda](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [IdVenda] INT FOREIGN KEY REFERENCES [dbo].[Vendas](Id),
        [IdProduto] INT FOREIGN KEY REFERENCES [dbo].[Produtos](Id),
        [Quantidade] INT NOT NULL,
        [ValorUnitario] DECIMAL(18,2) NOT NULL,
        [Subtotal] DECIMAL(18,2) NOT NULL
    );
END

-- Ordens de Serviço
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrdensServico]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OrdensServico](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [IdCliente] INT NOT NULL FOREIGN KEY REFERENCES [dbo].[Clientes](Id),
        [IdTecnico] INT NULL FOREIGN KEY REFERENCES [dbo].[Tecnicos](Id),
        [Marca] NVARCHAR(50) NULL,
        [Modelo] NVARCHAR(100) NULL,
        [Cor] NVARCHAR(30) NULL,
        [IMEI] NVARCHAR(50) NULL,
        [Defeito] NVARCHAR(MAX) NULL,
        [Servico] NVARCHAR(MAX) NULL,
        [ValorPecas] DECIMAL(18,2) DEFAULT 0,
        [ValorServico] DECIMAL(18,2) DEFAULT 0,
        [Desconto] DECIMAL(18,2) DEFAULT 0,
        [ValorTotal] DECIMAL(18,2) DEFAULT 0,
        [Status] NVARCHAR(50) DEFAULT 'Aberta',
        [DataAbertura] DATETIME DEFAULT GETDATE(),
        [DataAtualizacao] DATETIME DEFAULT GETDATE(),
        [DataConclusao] DATETIME NULL,
        [DataEntrega] DATETIME NULL
    );
END

-- Itens da OS (Peças usadas na OS)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OSItens]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OSItens](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [IdOS] INT FOREIGN KEY REFERENCES [dbo].[OrdensServico](Id),
        [IdProduto] INT FOREIGN KEY REFERENCES [dbo].[Produtos](Id),
        [Quantidade] INT NOT NULL,
        [ValorUnitario] DECIMAL(18,2) NOT NULL,
        [Subtotal] DECIMAL(18,2) NOT NULL
    );
END

-- Pagamentos (OS e Vendas Mistas)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pagamentos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Pagamentos](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [IdVenda] INT NULL FOREIGN KEY REFERENCES [dbo].[Vendas](Id),
        [IdOS] INT NULL FOREIGN KEY REFERENCES [dbo].[OrdensServico](Id),
        [Valor] DECIMAL(18,2) NOT NULL,
        [FormaPagamento] NVARCHAR(50) NOT NULL,
        [DataPagamento] DATETIME DEFAULT GETDATE()
    );
END

-- Fechamento / Fluxo de Caixa Diário
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Caixa]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Caixa](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Tipo] NVARCHAR(20) NOT NULL, -- ENTRADA/SAIDA
        [Valor] DECIMAL(18,2) NOT NULL,
        [Descricao] NVARCHAR(255) NOT NULL,
        [DataMovimentacao] DATETIME DEFAULT GETDATE(),
        [ReferenciaId] INT NULL, -- Id da Venda ou OS, opcional
        [TipoReferencia] NVARCHAR(20) NULL -- VENDA / OS
    );
END

-- INSERIR USUÁRIO ADMIN PADRÃO
IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Login] = 'admin')
BEGIN
    -- Senha 'admin' - Em produção deve ser HASH, aqui por simplificação do boilerplate, assumiremos 'admin' pra primeiro acesso
    INSERT INTO [dbo].[Usuarios] (Nome, Login, SenhaHash, NivelAcesso)
    VALUES ('Administrador', 'admin', 'admin', 'Admin');
END

PRINT 'Módulo de Banco de Dados criado e atualizado com sucesso!';
GO
