# 📚 FCG-Libraries

Projeto **FCG-Libraries** faz parte de um ecossistema de microsserviços voltado para gerenciamento de bibliotecas de jogos e usuários.  
Ele foi desenvolvido com foco em **event sourcing**, **mensageria assíncrona** e **boas práticas de arquitetura distribuída**.

---

## 🚀 Tecnologias Utilizadas
- **.NET 8 / ASP.NET Core** → API
- **Entity Framework Core** → persistência e abstração de acesso ao banco de dados SQL Server.
- **MongoDB** → armazenamento de eventos (Event Store).
- **Azure Service Bus** → mensageria baseada em tópicos e subscriptions.
- **Swagger / Swashbuckle** → documentação interativa da API.

---

## 🏛️ Arquitetura
- **Microsserviços** → cada contexto (Users, Games, Libraries, Payments) é isolado e independente.
- **Event-Driven Architecture** → comunicação entre serviços via eventos publicados em tópicos do Service Bus.
- **Event Sourcing** → todas as mudanças de estado são registradas como eventos imutáveis.
- **CQRS (Command Query Responsibility Segregation)** → separação clara entre comandos (alteração de estado) e queries (leitura de dados).
- **Camadas bem definidas**:
  - **API** → exposição dos endpoints REST.
  - **Application** → regras de negócio e orquestração de serviços.
  - **Infrastructure** → persistência, mensageria e integração externa.
  - **Domain** → entidades e lógica de domínio.

---

## 📐 Padrões e Designs
- **Repository Pattern** → abstração do acesso a dados.
- **Dependency Injection** → desacoplamento e facilidade de testes.
- **Middleware personalizado** → tratamento global de exceções e correlação de requisições.
- **Event Publisher/Consumer** → implementação de produtores e consumidores de eventos no Azure Service Bus.
- **Idempotência** → prevenção de duplicidade no processamento de eventos.

---

## 🔄 Fluxo de Eventos
1. **Criação/remoção de itens da biblioteca** gera um evento (`LibraryItemCreated`, `LibraryItemRemoved`).  
2. O evento é persistido no **MongoDB (Event Store)**.  
3. O evento é publicado no **Azure Service Bus (libraries-topic)**.  
4. O **Consumer** escuta os tópicos de `Libraries`, `Users` e `Games`:
   - Se um **User** for removido → todas as bibliotecas vinculadas a ele são apagadas.
   - Se um **Game** for removido → ele é removido de todas as bibliotecas.

---

## 📊 Observabilidade
- **Logs estruturados** com `CorrelationId` para rastrear requisições e eventos.  
- **Swagger** para documentação e testes de endpoints.  
- **GlobalExceptionMiddleware** para captura e padronização de erros.

---

## 🎯 Objetivo
Este projeto foi desenvolvido como parte de um portfólio pessoal para demonstrar:
- Conhecimento em **arquitetura de microsserviços**.  
- Aplicação prática de **event sourcing** e **mensageria assíncrona**.  
- Uso de **padrões de projeto** e boas práticas de engenharia de software.  

---

## 👨‍💻 Autor
**Alexandre Zordan Durães**  
Desenvolvedor focado em arquitetura distribuída, microsserviços e boas práticas de software.  
Este projeto faz parte de um portfólio para demonstrar experiência prática em tecnologias modernas.