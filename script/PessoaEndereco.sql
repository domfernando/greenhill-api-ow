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

CREATE TABLE `PessoaEndereco` (
  `ID` int(11) NOT NULL,
  `PessoaId` int(11) NOT NULL,
  `EnderecoId` int(11) NOT NULL,
  `Criado` datetime(6) DEFAULT NULL,
  `Alterado` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Índices de tabela `PessoaEndereco`
--
ALTER TABLE `PessoaEndereco`
  ADD PRIMARY KEY (`ID`),
  ADD KEY `FK_PessoaEndereco_Pessoa_PessoaId` (`PessoaId`),
  ADD KEY `IX_PessoaEndereco_EnderecoId` (`EnderecoId`);

--
-- AUTO_INCREMENT para tabelas despejadas
--

--
-- AUTO_INCREMENT de tabela `UsuarioPerfil`
--
ALTER TABLE `PessoaEndereco`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1;

--
-- Restrições para tabelas despejadas
--

--
-- Restrições para tabelas `UsuarioPerfil`
--
ALTER TABLE `PessoaEndereco`
  ADD CONSTRAINT `FK_PessoaEndereco_Endereco_EnderecoId` FOREIGN KEY (`EnderecoId`) REFERENCES `Endereco` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_PessoaEndereco_Pessoa_PessoaId` FOREIGN KEY (`PessoaId`) REFERENCES `Pessoa` (`id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
