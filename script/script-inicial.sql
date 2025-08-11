-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Tempo de geração: 09/07/2025 às 14:55
-- Versão do servidor: 10.11.10-MariaDB-log
-- Versão do PHP: 7.2.34

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Banco de dados: `u979250774_unit_api`
--

-- --------------------------------------------------------

--
-- Estrutura para tabela `Endereco`
--

CREATE TABLE `Endereco` (
  `ID` int(11) NOT NULL,
  `TipoEndereco` int(11) DEFAULT 1,
  `Logradouro` varchar(100) NOT NULL,
  `Numero` varchar(50) NOT NULL,
  `Complemento` varchar(100) DEFAULT NULL,
  `Bairro` varchar(100) DEFAULT NULL,
  `Cidade` varchar(100) DEFAULT NULL,
  `Estado` varchar(2) DEFAULT NULL,
  `CEP` varchar(12) DEFAULT NULL,
  `Criado` datetime DEFAULT NULL,
  `Alterado` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Despejando dados para a tabela `Endereco`
--

INSERT INTO `Endereco` (`ID`, `TipoEndereco`, `Logradouro`, `Numero`, `Complemento`, `Bairro`, `Cidade`, `Estado`, `CEP`, `Criado`, `Alterado`) VALUES
(1, 1, 'Rua José Maciel Neto', '315', 'AP 212 B', 'Jardim Maria Rosa', 'Taboão da Serra', 'SP', '06764040', '2025-07-04 19:53:18', '2025-07-05 11:05:04');

-- --------------------------------------------------------

--
-- Estrutura para tabela `Mensagem`
--

CREATE TABLE `Mensagem` (
  `ID` int(11) NOT NULL,
  `Nome` varchar(50) NOT NULL,
  `Conteudo` longtext DEFAULT NULL,
  `MessageMode` int(11) NOT NULL DEFAULT 1,
  `Criado` datetime(6) DEFAULT NULL,
  `Alterado` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `Mensagem`
--

INSERT INTO `Mensagem` (`ID`, `Nome`, `Conteudo`, `MessageMode`, `Criado`, `Alterado`) VALUES
(1, 'Token Sucesso (Whatsapp)', '<p> Olá @Nome </p>\n<p><b>Seu token: </b> \n<i> @Token </i></p>', 1, NOW(), NULL),
(2, 'Aviso (Email)', NULL, 2, NOW(), NULL),
(3, 'Teste (wats)', NULL, 1, NOW(), NULL);

-- --------------------------------------------------------

--
-- Estrutura para tabela `Papel`
--

CREATE TABLE `Papel` (
  `ID` int(11) NOT NULL,
  `Nome` varchar(50) NOT NULL,
  `Ativo` tinyint(1) NOT NULL,
  `Criado` datetime DEFAULT NULL,
  `Alterado` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Despejando dados para a tabela `Papel`
--

INSERT INTO `Papel` (`ID`, `Nome`, `Ativo`, `Criado`, `Alterado`) VALUES
(1, 'Cliente', 1, NOW(), NULL),
(2, 'Fornecedor', 1, NOW(), NULL);

-- --------------------------------------------------------

--
-- Estrutura para tabela `Perfil`
--

CREATE TABLE `Perfil` (
  `id` int(11) NOT NULL,
  `nome` varchar(255) NOT NULL,
  `Criado` datetime DEFAULT NULL,
  `Alterado` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Despejando dados para a tabela `Perfil`
--

INSERT INTO `Perfil` (`id`, `nome`, `Criado`, `Alterado`) VALUES
(1, 'Master', NOW(), NULL),
(2, 'Admin', NOW(), NULL),
(3, 'Operador', NOW(), NULL);

-- --------------------------------------------------------

--
-- Estrutura para tabela `Pessoa`
--

CREATE TABLE `Pessoa` (
  `ID` int(11) NOT NULL,
  `Nome` varchar(50) NOT NULL,
  `NomeCompleto` varchar(100) DEFAULT NULL,
  `Fisica` tinyint(1) DEFAULT 1,
  `Telefone` varchar(50) DEFAULT NULL,
  `Celular` varchar(50) DEFAULT NULL,
  `Email` varchar(200) DEFAULT NULL,
  `Documento` varchar(50) DEFAULT NULL,
  `SituacaoComercial` int(11) DEFAULT 1,
  `Nascimento` datetime DEFAULT NULL,
  `Observacao` longtext DEFAULT NULL,
  `Criado` datetime DEFAULT NULL,
  `Alterado` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Despejando dados para a tabela `Pessoa`
--

INSERT INTO `Pessoa` (`ID`, `Nome`, `NomeCompleto`, `Fisica`, `Telefone`, `Celular`, `Email`, `Documento`, `SituacaoComercial`, `Nascimento`, `Observacao`, `Criado`, `Alterado`) VALUES
(1, 'Fernando Domingues', 'Fernando Bento Domingues', 1, NULL, '11987631288', 'fdomingues@hotmail.com', '', 1, '1972-10-08 00:00:00', 'Teste', NOW(), NULL);

-- --------------------------------------------------------

--
-- Estrutura para tabela `PessoaEndereco`
--

CREATE TABLE `PessoaEndereco` (
  `ID` int(11) NOT NULL,
  `PessoaId` int(11) NOT NULL,
  `EnderecoId` int(11) NOT NULL,
  `Criado` datetime(6) DEFAULT NULL,
  `Alterado` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `PessoaEndereco`
--

INSERT INTO `PessoaEndereco` (`ID`, `PessoaId`, `EnderecoId`, `Criado`, `Alterado`) VALUES
(1, 1, 1, NOW(), NULL);

-- --------------------------------------------------------

--
-- Estrutura para tabela `PessoaPapel`
--

CREATE TABLE `PessoaPapel` (
  `ID` int(11) NOT NULL,
  `PessoaId` int(11) NOT NULL,
  `PapelId` int(11) NOT NULL,
  `Criado` datetime(6) DEFAULT NULL,
  `Alterado` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `PessoaPapel`
--

INSERT INTO `PessoaPapel` (`ID`, `PessoaId`, `PapelId`, `Criado`, `Alterado`) VALUES
(1, 1, 1, NOW(), NULL);

-- --------------------------------------------------------

--
-- Estrutura para tabela `Queue`
--

CREATE TABLE `Queue` (
  `ID` int(11) NOT NULL,
  `Source` varchar(200) DEFAULT NULL,
  `MessageMode` int(11) DEFAULT 1,
  `Instance` varchar(50) NOT NULL,
  `Name` varchar(50) DEFAULT NULL,
  `Address` varchar(200) DEFAULT NULL,
  `Message` longtext DEFAULT NULL,
  `SendDate` datetime(6) DEFAULT NULL,
  `processed` tinyint(1) DEFAULT 0,
  `enabled` tinyint(1) DEFAULT 0,
  `success` tinyint(1) DEFAULT 0,
  `attempts` int(11) DEFAULT NULL,
  `log` longtext DEFAULT NULL,
  `Criado` datetime(6) DEFAULT NULL,
  `Alterado` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Despejando dados para a tabela `Queue`
--

INSERT INTO `Queue` (`ID`, `Source`, `MessageMode`, `Instance`, `Name`, `Address`, `Message`, `SendDate`, `processed`, `enabled`, `success`, `attempts`, `log`, `Criado`, `Alterado`) VALUES
(1, 'Autenticação MFA', 1, 'NandoBento', 'Fernando', '5511987631288', 'Olá\r\n*Seu token:* \r\n_PCL43C</>', '2025-07-03 11:22:16.000000', 1, 0, 0, 1, 'Whatsapp para Fernando 5511987631288 enviado com sucesso.', '2025-07-03 11:22:16.608087', '2025-07-03 12:15:47.890463'),
(2, 'Autenticação MFA', 1, 'NandoBento', 'Nando', '5511987631288', 'Olá Fernando Domingues <br/>\n*Seu token:* \n_YPE21Q</>', '2025-07-03 11:24:19.000000', 1, 0, 0, 1, 'Whatsapp para Nando 5511987631288 enviado com sucesso.', '2025-07-03 11:24:36.105994', '2025-07-03 12:16:01.517413'),
(3, 'Autenticação MFA', 1, 'NandoBento', 'Fernando Domingues', '5511987631288', 'Olá Fernando Domingues <br/>\n*Seu token:* \n_XN6EDE</>', '2025-07-03 11:27:43.000000', 1, 0, 1, 1, 'Whatsapp para Fernando Domingues 5511987631288 enviado com sucesso.', '2025-07-03 11:27:43.326649', '2025-07-03 12:16:03.435403'),
(4, 'Autenticação MFA', 1, 'NandoBento', 'Fernando Domingues', '5511987631288', 'Olá Fernando Domingues \n\n*Seu token:* \n_QNUASK_', '2025-07-03 12:22:43.000000', 1, 0, 0, 1, 'Whatsapp para Fernando Domingues 5511987631288 enviado com sucesso.', '2025-07-03 12:22:43.824253', '2025-07-03 12:23:50.140166'),
(5, 'Autenticação MFA', 1, 'NandoBento', 'Fernando Domingues', '5511987631288', 'Olá Fernando Domingues \n\n*Seu token:* \n_3D5RPN_', '2025-07-03 12:26:30.000000', 1, 0, 0, 1, 'Whatsapp para Fernando Domingues 5511987631288 enviado com sucesso.', '2025-07-03 12:26:30.707666', '2025-07-03 12:27:51.913118'),
(6, 'Autenticação MFA', 1, 'NandoBento', 'Fernando Domingues', '5511987631288', 'Olá Fernando Domingues \n\n*Seu token:* \n_ALMAM8_', '2025-07-03 12:26:34.000000', 1, 0, 0, 1, 'Whatsapp para Fernando Domingues 5511987631288 enviado com sucesso.', '2025-07-03 12:26:34.250004', '2025-07-03 12:27:51.935580'),
(7, 'Autenticação MFA', 1, 'NandoBento', 'Fernando Domingues', '5511987631288', ' Olá Fernando Domingues \n\n*Seu token: * \n_ T4BERO _\n', '2025-07-03 12:30:37.000000', 1, 0, 0, 1, 'Whatsapp para Fernando Domingues 5511987631288 enviado com sucesso.', '2025-07-03 12:30:37.498813', '2025-07-03 12:31:27.744017'),
(8, 'Autenticação MFA', 1, 'NandoBento', 'Fernando Domingues', '5511987631288', ' Olá Fernando Domingues \n\n*Seu token: * \n_ FH7NQD _\n', '2025-07-03 12:32:40.000000', 1, 0, 0, 1, 'Whatsapp para Fernando Domingues 5511987631288 enviado com sucesso.', '2025-07-03 12:32:40.438513', '2025-07-03 12:33:15.391490');

-- --------------------------------------------------------

--
-- Estrutura para tabela `Servico`
--

CREATE TABLE `Servico` (
  `ID` int(11) NOT NULL,
  `Nome` varchar(50) NOT NULL,
  `Valor` longtext DEFAULT NULL,
  `Ativo` tinyint(1) NOT NULL DEFAULT 1,
  `Criado` datetime(6) DEFAULT NULL,
  `Alterado` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `Servico`
--

INSERT INTO `Servico` (`ID`, `Nome`, `Valor`, `Ativo`, `Criado`, `Alterado`) VALUES
(1, 'EMAIL', '{\"Name\":\"Email\",\"Url\": \"smtpi.kinghost.net\",\"Port\": \"587\",\"User\": \"ebook@vfkeducacao.com\",\"Password\": \"Pdf@Changer2023\",\"CopyTo\": \"fdomingues@hotmail.com\",\"Sender\":\"fdomingues@gmail.com\"}', 1, NOW(), NULL),
(2, 'Evolution', '{\"Name\":\"Evolution\",\"Url\": \"https://notificador.webnando.com.br\",\"Port\": \"\",\"User\": \"\",\"Password\": \"\",\"Token\": \"tooAFpCmQQByqtDWTtXxUhi31JHj0UnT\"}', 1, NOW(), NULL),
(3, 'CEP', '{\"Name\":\"ViaCep\",\"Url\": \"https://viacep.com.br/ws\",\"Port\": \"\",\"User\": \"\",\"Password\": \"\",\"Token\": \"\"}', 1, NOW(), NULL);

-- --------------------------------------------------------

--
-- Estrutura para tabela `Usuario`
--

CREATE TABLE `Usuario` (
  `id` int(11) NOT NULL,
  `nome` varchar(255) DEFAULT NULL,
  `nomecompleto` varchar(255) DEFAULT NULL,
  `email` varchar(255) NOT NULL,
  `ddi` int(11) DEFAULT NULL,
  `celular` varchar(255) DEFAULT NULL,
  `whatsapp` varchar(50) DEFAULT NULL,
  `usr` varchar(255) DEFAULT NULL,
  `pwd` varchar(255) DEFAULT NULL,
  `codigo` varchar(255) DEFAULT NULL,
  `Mfa` tinyint(1) DEFAULT NULL,
  `MfaModo` varchar(50) DEFAULT NULL,
  `MfaCodigo` varchar(50) DEFAULT NULL,
  `MfaExpira` datetime DEFAULT NULL,
  `ativo` tinyint(1) DEFAULT NULL,
  `travado` varchar(255) DEFAULT NULL,
  `verificado` varchar(50) DEFAULT NULL,
  `tentativa` int(11) DEFAULT NULL,
  `ultimologin` varchar(255) DEFAULT NULL,
  `Criado` datetime DEFAULT NULL,
  `Alterado` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_general_ci;

--
-- Despejando dados para a tabela `Usuario`
--

INSERT INTO `Usuario` (`id`, `nome`, `nomecompleto`, `email`, `ddi`, `celular`, `whatsapp`, `usr`, `pwd`, `codigo`, `Mfa`, `MfaModo`, `MfaCodigo`, `MfaExpira`, `ativo`, `travado`, `verificado`, `tentativa`, `ultimologin`, `Criado`, `Alterado`) VALUES
(1, 'Fernando Domingues', 'Fernando Bento Domingues', 'fdomingues@hotmail.com', NULL, '11987631288', NULL, 'fdomingues@hotmail.com', '000700550060002200d4004d00d3006b00f6001f0010002100b0000600b000ae', '54YST6', 0, 'Whatsapp', '', NULL, 1, '', '08/10/1972', 0, '09/07/2025 11:27:48', NOW(),NULL);

-- --------------------------------------------------------

--
-- Estrutura para tabela `UsuarioPerfil`
--

CREATE TABLE `UsuarioPerfil` (
  `ID` int(11) NOT NULL,
  `UsuarioId` int(11) NOT NULL,
  `PerfilId` int(11) NOT NULL,
  `Criado` datetime(6) DEFAULT NULL,
  `Alterado` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `UsuarioPerfil`
--

INSERT INTO `UsuarioPerfil` (`ID`, `UsuarioId`, `PerfilId`, `Criado`, `Alterado`) VALUES
(1, 1, 1, NOW(), NULL);

--
-- Índices para tabelas despejadas
--

--
-- Índices de tabela `Endereco`
--
ALTER TABLE `Endereco`
  ADD PRIMARY KEY (`ID`);

--
-- Índices de tabela `Mensagem`
--
ALTER TABLE `Mensagem`
  ADD PRIMARY KEY (`ID`);

--
-- Índices de tabela `Papel`
--
ALTER TABLE `Papel`
  ADD PRIMARY KEY (`ID`);

--
-- Índices de tabela `Perfil`
--
ALTER TABLE `Perfil`
  ADD PRIMARY KEY (`id`);

--
-- Índices de tabela `Pessoa`
--
ALTER TABLE `Pessoa`
  ADD PRIMARY KEY (`ID`);

--
-- Índices de tabela `PessoaEndereco`
--
ALTER TABLE `PessoaEndereco`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `FK_PessoaEndereco_Pessoa_PessoaId` (`PessoaId`),
  ADD KEY `IX_PessoaEndereco_EnderecoId` (`EnderecoId`);

--
-- Índices de tabela `PessoaPapel`
--
ALTER TABLE `PessoaPapel`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `FK_PessoaPapel_Pessoa_PessoaId` (`PessoaId`),
  ADD KEY `IX_PessoaPapel_PapelId` (`PapelId`);

--
-- Índices de tabela `Queue`
--
ALTER TABLE `Queue`
  ADD PRIMARY KEY (`ID`);

--
-- Índices de tabela `Servico`
--
ALTER TABLE `Servico`
  ADD PRIMARY KEY (`ID`);

--
-- Índices de tabela `Usuario`
--
ALTER TABLE `Usuario`
  ADD PRIMARY KEY (`id`);

--
-- Índices de tabela `UsuarioPerfil`
--
ALTER TABLE `UsuarioPerfil`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `FK_UsuarioPerfil_Usuario_UsuarioId` (`UsuarioId`),
  ADD KEY `IX_UsuarioPerfil_PerfilId` (`PerfilId`);

--
-- AUTO_INCREMENT para tabelas despejadas
--

--
-- AUTO_INCREMENT de tabela `Endereco`
--
ALTER TABLE `Endereco`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de tabela `Mensagem`
--
ALTER TABLE `Mensagem`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de tabela `Papel`
--
ALTER TABLE `Papel`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de tabela `Perfil`
--
ALTER TABLE `Perfil`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de tabela `Pessoa`
--
ALTER TABLE `Pessoa`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de tabela `PessoaEndereco`
--
ALTER TABLE `PessoaEndereco`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de tabela `PessoaPapel`
--
ALTER TABLE `PessoaPapel`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de tabela `Queue`
--
ALTER TABLE `Queue`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT de tabela `Servico`
--
ALTER TABLE `Servico`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT de tabela `Usuario`
--
ALTER TABLE `Usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT de tabela `UsuarioPerfil`
--
ALTER TABLE `UsuarioPerfil`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Restrições para tabelas despejadas
--

--
-- Restrições para tabelas `PessoaEndereco`
--
ALTER TABLE `PessoaEndereco`
  ADD CONSTRAINT `FK_PessoaEndereco_Endereco_EnderecoId` FOREIGN KEY (`EnderecoId`) REFERENCES `Endereco` (`ID`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_PessoaEndereco_Pessoa_PessoaId` FOREIGN KEY (`PessoaId`) REFERENCES `Pessoa` (`ID`) ON DELETE CASCADE;

--
-- Restrições para tabelas `PessoaPapel`
--
ALTER TABLE `PessoaPapel`
  ADD CONSTRAINT `FK_PessoaPapel_Papel_PapelId` FOREIGN KEY (`PapelId`) REFERENCES `Papel` (`ID`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_PessoaPapel_Pessoa_PessoaId` FOREIGN KEY (`PessoaId`) REFERENCES `Pessoa` (`ID`) ON DELETE CASCADE;

--
-- Restrições para tabelas `UsuarioPerfil`
--
ALTER TABLE `UsuarioPerfil`
  ADD CONSTRAINT `FK_UsuarioPerfil_Perfil_PerfilId` FOREIGN KEY (`PerfilId`) REFERENCES `Perfil` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_UsuarioPerfil_Usuario_UsuarioId` FOREIGN KEY (`UsuarioId`) REFERENCES `Usuario` (`id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
