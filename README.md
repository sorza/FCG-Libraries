# 📚 FCG-Libraries

Microsserviço de Bibliotecas de Jogos — Gerenciamento com sincronização cross-service via eventos.

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-green)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![Event-Driven](https://img.shields.io/badge/Architecture-Event--Driven-orange)](https://martinfowler.com/articles/201701-event-driven.html)
[![Choreography](https://img.shields.io/badge/Pattern-Choreography-red)](https://microservices.io/patterns/data/saga.html)

## 📝 Descrição

**FCG-Libraries** gerencia bibliotecas pessoais de jogos:

- ✅ **Coleção por usuário**: Games salvos na biblioteca pessoal
- ✅ **Sincronização automática**: Reage a eventos de Games, Payments, Users
- ✅ **Choreography**: Sem orquestrador central, cada serviço age independentemente
- ✅ **Autorização**: Usuários comuns veem sua biblioteca, Admins veem todas
- ✅ **Event Sourcing**: Histórico imutável de alterações

---

## 🚀 Pré-requisitos

- .NET 8 SDK
- SQL Server
- MongoDB (Event Store)
- Azure Service Bus
- Docker (opcional)

---

## ⚙️ Configuração Local

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LibrariesDb;Trusted_Connection=True;"
  },
  "ServiceBus": {
    "ConnectionString": "<connection-string>",
    "Topics": { "Libraries": "libraries-events" }
  },
  "MongoSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "EventStoreDb"
  },
  "Jwt": {
    "Key": "9y4XJg0aTphzFJw3TvksRvqHXd+Q4VB8f7ZvU08N+9Q=",
    "Issuer": "FGC-Users"
  }
}
```

---

## 🚀 Como Executar

### Migrations
```bash
cd FCG-Libraries.Api
dotnet ef database update
```

### API
```bash
cd FCG-Libraries.Api
dotnet run
# https://localhost:7004/swagger
```

### Consumer (Multi-topic)
```bash
cd FCG-Libraries.Consumer
dotnet run
# Escuta: games-events, payments-events, users-topic
```

---

## 📊 Endpoints

| Método | Endpoint   | Autenticação | Descrição |
|--------|------------|--------------|-----------|
| GET    | `/api`     | Qualquer     | Sua biblioteca |
| GET    | `/api/all` | Admin        | Todas as bibliotecas |
| POST   | `/api`     | Qualquer     | Adicionar jogo |
| PUT    | `/api/{id}`| Qualquer     | Atualizar item |
| DELETE | `/api/{id}`| Admin        | Remover item |

---

## 🧪 Testar

```bash
# Adicionar jogo à biblioteca
curl -X POST https://localhost:7004/api \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "gameId": "7b8c9d0e-1f2a-3b4c-5d6e-7f8a9b0c1d2e",
    "isFavorite": false,
    "isInstalled": false
  }'
```

---

## 🐳 Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FCG-Libraries.Api/", "."]
RUN dotnet restore && dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "FCG-Libraries.Api.dll"]
```

---

## ☸️ Kubernetes

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: fcg-libraries
spec:
  replicas: 2
  selector:
    matchLabels:
      app: fcg-libraries
  template:
    metadata:
      labels:
        app: fcg-libraries
    spec:
      containers:
      - name: fcg-libraries
        image: fcg-libraries:latest
        ports:
        - containerPort: 8080
```

**HPA**:
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: fcg-libraries-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: fcg-libraries
  minReplicas: 1
  maxReplicas: 5
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```

---

## 📚 Referências

- [Choreography Pattern](https://microservices.io/patterns/data/saga.html)
- [Event-Driven Architecture](https://martinfowler.com/articles/201701-event-driven.html)
- [DDD](https://www.domainlanguage.com/ddd/)

---

**FIAP Tech Challenge — Fase 4**
