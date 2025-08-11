-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Tempo de geração: 04/07/2025 às 19:47
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
(1, 1, 1, '2025-07-02 15:16:50.000000', NULL);

--
-- Índices para tabelas despejadas
--

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
-- AUTO_INCREMENT de tabela `UsuarioPerfil`
--
ALTER TABLE `UsuarioPerfil`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Restrições para tabelas despejadas
--

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
