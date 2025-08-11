-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Tempo de geração: 01/07/2025 às 20:45
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
-- Estrutura para tabela `Servico`
--

CREATE TABLE `Servico` (
  `ID` int(11) NOT NULL,
  `Nome` varchar(50) NOT NULL,
  `Valor` longtext DEFAULT NULL,
  `Criado` datetime(6) DEFAULT NULL,
  `Alterado` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `Servico`
--

INSERT INTO `Servico` (`ID`, `Nome`, `Valor`, `Criado`, `Alterado`) VALUES
(1, 'EMAIL', '{\"Domain\": \"smtpi.kinghost.net\",\"Port\": \"587\",\"Sender\": \"ebook@vfkeducacao.com\",\"Password\": \"Pdf@Changer2023\",\"CC\": \"fdomingues@hotmail.com\"}', '2024-02-15 10:35:01.000000', NULL),
(2, 'HUGGY', '{\"Nome\":\"HUGGY\",\"Url\": \"https://api.huggy.app/v2/\",\"Token\": \"Bearer a9cf9e0849ee771647bfd2ee2c71d487\"}', '2024-02-21 00:00:00.000000', NULL);

--
-- Índices para tabelas despejadas
--

--
-- Índices de tabela `Servico`
--
ALTER TABLE `Servico`
  ADD PRIMARY KEY (`ID`);

--
-- AUTO_INCREMENT para tabelas despejadas
--

--
-- AUTO_INCREMENT de tabela `Servico`
--
ALTER TABLE `Servico`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
