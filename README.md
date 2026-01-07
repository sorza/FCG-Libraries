# FCG-Libraries

Projeto **FCG-Libraries** faz parte de um ecossistema de microsserviços voltado para gerenciamento de bibliotecas de jogos e usuários.  
Ele foi desenvolvido com foco em **event sourcing**, **mensageria assíncrona** e **boas práticas de arquitetura distribuída**.

## Visão geral
- Plataforma: `.NET 8` (C#)
- Estrutura modular com camadas: `Api`, `Application`, `Domain`, `Infrastructure`, `Consumer`
- Comunicação assíncrona via Azure Service Bus
- Persistência com Entity Framework Core e SQL Server
- MongoDB para armazenamento de eventos (Event Store).
- Consumidores/Workers implementados como `BackgroundService` 

## Arquitetura e metodologias
- Clean Architecture / Onion Architecture
- Domain-Driven Design (DDD)
- Event-Driven Architecture
- Event Sourcing 
- CQRS 
- Microsserviços e comunicação assíncrona (tópicos/assinaturas)
- Projetado para containerização e CI/CD
- Idempotência

## Padrões de projeto
- Dependency Injection (padrão do .NET)
- Repository Pattern / Unit of Work (com EF Core)
- CQRS (quando aplicável)
- BackgroundService para processamento assíncrono
- DTOs e mappers entre camadas

## Estrutura do repositório
- `FCG-Libraries.Api` — API REST (Controllers)
- `FCG-Libraries.Application` — Casos de uso e serviços de aplicação
- `FCG-Libraries.Domain` — Entidades e interfaces de domínio
- `FCG-Libraries.Infrastructure` — Implementações (EF Core, Service Bus, DI)
- `FCG-Libraries.Consumer` — Consumers (ex.: `PaymentsTopicConsumer`, `UsersTopicConsumer`)

## Como executar (resumo)
1. Configurar connection strings / credenciais em `appsettings.Development.json` ou variáveis de ambiente. Para segredos locais, use __User Secrets__ ou serviços de segredo em nuvem.
2. Restaurar pacotes: `dotnet restore`
3. Atualizar banco (EF Core migrations): `dotnet ef database update`
4. Executar API: `dotnet run --project FCG-Libraries.Api`
5. Executar consumers/worker services conforme configuração (são implementados como `BackgroundService`)

> Observação: adaptar configurações para conexão com Azure Service Bus e SQL Server em ambiente local ou nuvem.

## Boas práticas recomendadas
- Não armazenar segredos no repositório — usar __User Secrets__, Azure Key Vault ou variáveis de ambiente.
- Instrumentar logs, métricas e tracing para observabilidade.

## SEO
.NET 8, C#, Clean Architecture, Onion Architecture, DDD, CQRS, Azure Service Bus, EF Core 8, Entity Framework, SQL Server, Dependency Injection, BackgroundService, Worker Service, Microservices, Azure, DevOps, GitHub, Portfolio, REST API, Messaging, Asynchronous Processing, Repository Pattern
