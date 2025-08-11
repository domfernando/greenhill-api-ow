-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Tempo de geração: 01/07/2025 às 13:37
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
-- Banco de dados: `u979250774_greenhill`
--

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
(1, 'Master', '2022-01-26 11:45:35', '2025-06-27 16:14:59'),
(2, 'Admin', '2022-01-26 11:45:35', NULL),
(3, 'Operador', '2022-01-26 11:45:35', NULL);

--
-- Índices para tabelas despejadas
--

--
-- Índices de tabela `Perfil`
--
ALTER TABLE `Perfil`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT para tabelas despejadas
--

--
-- AUTO_INCREMENT de tabela `Perfil`
--
ALTER TABLE `Perfil`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
