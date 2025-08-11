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
  `social` tinyint(1) DEFAULT NULL,
  `socialtipo` varchar(255) DEFAULT NULL,
  `socialid` varchar(255) DEFAULT NULL,
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

INSERT INTO `Usuario` (`id`, `nome`, `nomecompleto`, `email`, `ddi`, `celular`, `whatsapp`, `usr`, `pwd`, `codigo`, `social`, `socialtipo`, `socialid`, `ativo`, `travado`, `verificado`, `tentativa`, `ultimologin`, `Criado`, `Alterado`) VALUES
(1, 'Fernando Domingues', 'Fernando Bento Domingues', 'fdomingues@hotmail.com', NULL, '11987631288', NULL, 'fdomingues@hotmail.com', '000700550060002200d4004d00d3006b00f6001f0010002100b0000600b000ae', '8YTQLM', 0, '', '', 1, NULL, '08/10/1972', 0, '30/06/2025 14:24:17', '2022-01-26 11:45:35', '2025-06-26 19:33:43'),
(5, 'Nando Bento', 'Fernando Bento', 'fdomingues@gmail.com', NULL, '11987631288', NULL, 'fdomingues@gmail.com', NULL, '3V6MHV', NULL, NULL, NULL, 1, '', '', NULL, NULL, '2025-06-27 15:18:07', '2025-06-27 15:18:58'),
(6, 'Sabrina Rios', 'Sabrina Rios Domingues', 'sabrinard1976@gmail.com', NULL, '(11)98610-2055', NULL, 'sabrinard1976@gmail.com', '000700550060002200d4004d00d3006b00f6001f0010002100b0000600b000ae', 'AFHTC5', NULL, NULL, NULL, 1, '', '', NULL, NULL, '2025-06-27 15:35:07', '2025-06-27 15:37:32');

--
-- Índices para tabelas despejadas
--

--
-- Índices de tabela `Usuario`
--
ALTER TABLE `Usuario`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT para tabelas despejadas
--

--
-- AUTO_INCREMENT de tabela `Usuario`
--
ALTER TABLE `Usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
