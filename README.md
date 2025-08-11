# Unit.API

## Visão Geral

Este projeto é uma API desenvolvida em .NET 8, utilizando C# 12, que expõe endpoints para gerenciamento de usuários, filas de mensagens, perfis, serviços e integrações externas. A arquitetura segue boas práticas de separação de camadas, utilizando injeção de dependência, FluentValidation e Entity Framework Core.

## Uso das Controllers

As controllers são responsáveis por expor os endpoints HTTP para interação com os recursos do sistema. Abaixo estão as principais controllers e seus propósitos:

### 1. **UsuarioController**
Gerencia operações relacionadas a usuários do sistema.
- **POST /api/usuario**: Cria um novo usuário.
- **GET /api/usuario/{id}**: Retorna os dados de um usuário específico.
- **PUT /api/usuario/{id}**: Atualiza informações de um usuário.
- **DELETE /api/usuario/{id}**: Remove um usuário do sistema.

### 2. **QueueController**
Gerencia a fila de mensagens para processamento assíncrono.
- **POST /api/queue**: Adiciona uma nova mensagem à fila.
- **GET /api/queue**: Lista todas as mensagens da fila.
- **GET /api/queue/{id}**: Retorna detalhes de uma mensagem específica.
- **POST /api/queue/process**: Processa mensagens pendentes na fila.

### 3. **PerfilController**
Gerencia perfis de usuários.
- **GET /api/perfil**: Lista todos os perfis.
- **POST /api/perfil**: Cria um novo perfil.
- **PUT /api/perfil/{id}**: Atualiza um perfil existente.
- **DELETE /api/perfil/{id}**: Remove um perfil.

### 4. **ServicoController**
Gerencia serviços disponíveis no sistema.
- **GET /api/servico**: Lista todos os serviços.
- **POST /api/servico**: Cria um novo serviço.
- **PUT /api/servico/{id}**: Atualiza um serviço.
- **DELETE /api/servico/{id}**: Remove um serviço.

### 5. **EvolutionController**
Integração com serviços externos, como envio de mensagens via WhatsApp.

## EvolutionService

O **EvolutionService** é responsável por integrar a API com plataformas externas de mensageria, como o WhatsApp. Ele permite o envio automatizado de mensagens para contatos cadastrados, sendo utilizado principalmente pelo sistema de filas (QueueService) para processar e disparar notificações ou comunicações automáticas. Os métodos do EvolutionService são consumidos internamente pelas controllers e serviços, facilitando a comunicação assíncrona e a automação de processos de envio de mensagens.

## Como Usar

1. **Configuração:** Ajuste as configurações de conexão e variáveis de ambiente no `appsettings.json`.
2. **Execução:** Execute o projeto via Visual Studio ou CLI (`dotnet run`).
3. **Documentação:** Utilize o Swagger (normalmente disponível em `/swagger`) para explorar e testar os endpoints.

## Observações

- Todas as rotas seguem o padrão RESTful.
- A validação de dados é feita via FluentValidation.
- O projeto utiliza injeção de dependência para todos os serviços e repositórios.

---

Para dúvidas ou contribuições, consulte a documentação interna ou entre em contato com o time de desenvolvimento.