-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Tempo de geração: 02/07/2025 às 20:13
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
-- Estrutura para tabela `Mensagem`
--

CREATE TABLE `Mensagem` (
  `ID` int(11) NOT NULL,
  `Nome` varchar(50) NOT NULL,
  `Conteudo` longtext DEFAULT NULL,
  `Tipo` int(11) NOT NULL,
  `Criado` datetime(6) DEFAULT NULL,
  `Alterado` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Despejando dados para a tabela `Mensagem`
--

INSERT INTO `Mensagem` (`ID`, `Nome`, `Conteudo`, `Tipo`, `Criado`, `Alterado`) VALUES
(1, 'Aviso designação', '<p>Olá, irmão   @Nome</p><p><br></p><p>Sua designação se aproxima</p><p><strong>Data:</strong> @Data</p><p><br></p><p>@Conteudo</p>', 1, NULL, '2024-12-06 19:00:20.450305'),
(2, 'Lembrete de arranjo', '<p>Olá irmão @Nome, como vai?</p><p>Temos um arranjo de oradores agendado para  <strong>@Mes</strong></p><p>@Conteudo</p><p><strong>Por favor, me informe:</strong></p><p><br></p><p>- Está confirmado?</p><p>- Quantas datas serão necessárias?</p><p>- Vocês priorizam apenas anciãos ou podemos enviar servos ministeriais também?</p><p>Assim que o irmão me confirmar essas informações trocamos a lista dos oradores e temas, ok?</p><p><br></p><p><strong>Forte abraço</strong></p>', 1, '2024-09-16 15:34:09.000000', '2024-12-10 10:20:41.642279'),
(3, 'Discurso Confirmação', '<p>Olá, @Nome</p><p><br></p><p>Gostaria de confirmar se será possível cumprir essa designação:</p><p>@Conteudo</p><p><br></p><p>Por favor, responda o quanto antes</p><p><strong>Obrigado</strong></p>', 1, '2024-09-16 15:34:09.000000', '2024-12-06 18:44:41.671065'),
(4, 'Discurso Dados ', '<p>Olá irmão,</p><p>Está tudo certo para a designação abaixo?</p><p>@Conteudo</p><p>Se estiver, por favor, envie essas informações:</p><p>- Cântico</p><p>- As imagens (caso for utilizar)</p><p>- Ficará até o final da reunião?</p><p><br></p><p>Muito obrigado.&nbsp;</p><p>Que Jeová continue abençoando seus esforços!</p>', 0, '2024-12-06 12:04:40.493583', '2024-12-06 19:11:08.270967'),
(5, 'Anúncio Arranjo', '<p>Olá</p><p>Fechamos um arranjo de oradores conforme abaixo:</p><p>@Conteudo</p>', 0, '2024-12-06 14:29:56.096502', '2024-12-06 16:02:18.100429');

--
-- Índices para tabelas despejadas
--

--
-- Índices de tabela `Mensagem`
--
ALTER TABLE `Mensagem`
  ADD PRIMARY KEY (`ID`);

--
-- AUTO_INCREMENT para tabelas despejadas
--

--
-- AUTO_INCREMENT de tabela `Mensagem`
--
ALTER TABLE `Mensagem`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
