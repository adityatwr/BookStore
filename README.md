# BookStore Microservices (.NET, Docker, RabbitMQ, MassTransit)

A minimal **microservices** built with **C#/.NET**, designed to be clean, testable, and production-friendly. It includes:

- **Book Catalogue API** — create/list books and publish `BookAdded`.
- **Order Service API** — place orders, validates against Book Catalogue, publishes `OrderPlaced`.
- **Auditing Service (Worker)** — consumes `BookAdded` / `OrderPlaced` and logs them.
- **RabbitMQ** for async messaging (AMQP) via **MassTransit**.
- **Repository pattern** (in-memory now; easy to swap).
- **Swagger/OpenAPI** for both HTTP services.

> Targets **.NET 9** by default. Containers are orchestrated with **Docker Compose**.

---

## Architecture

```mermaid
flowchart LR
  Client[API Client / Browser\nSwagger, Postman]:::ext -->|HTTP :7001| BookAPI[Book Catalogue API]
  Client -->|HTTP :7002| OrderAPI[Order Service API]

  subgraph Net["Docker network (compose)\nDNS: book-catalogue, order-service, rabbitmq"]
    BookAPI:::svc --> BookRepo[(Book Repository\nIn-memory ➜ EF Core)]:::db
    OrderAPI:::svc --> OrderRepo[(Order Repository\nIn-memory ➜ EF Core)]:::db
    AuditSvc[Auditing Service\n.NET Worker]:::svc
    Rabbit[(RabbitMQ Broker\nAMQP 5672 | Mgmt 15672)]:::mq
  end

  OrderAPI -- "GET /books/{id}\n(typed HttpClient)" --> BookAPI
  BookAPI -- "Publish BookAdded" --> Rabbit
  OrderAPI -- "Publish OrderPlaced" --> Rabbit
  AuditSvc -- "Consume both events" --> Rabbit

  classDef svc fill:#eef7ff,stroke:#3b82f6,stroke-width:1px,color:#0b3b8f;
  classDef db fill:#fff7e6,stroke:#f59e0b,stroke-width:1px;
  classDef mq fill:#fce7f3,stroke:#db2777,stroke-width:1px;
  classDef ext fill:#e5e7eb,stroke:#6b7280,color:#111827;
```

## Tech Stack

- **.NET** (C# 12): ASP.NET Core Web API & Worker
- **MassTransit** + **RabbitMQ**
- **Swagger / OpenAPI**
- **Docker & Docker Compose**
- **Repository pattern** with in-memory implementations

---

## Getting Started

### Prerequisites
- Docker Desktop
- .NET SDK (only if you want to run from VS / `dotnet run`)

### 1) Run with Docker Compose (recommended)
```bash
# from the repo root
docker compose up -d --build

# see status
docker compose ps

# tail logs (in another terminal)
docker compose logs -f rabbitmq
docker compose logs -f book-catalogue
docker compose logs -f order-service
docker compose logs -f auditing-service
```

**Default ports**

| Service              | URL / Port                 |
|----------------------|----------------------------|
| Book Catalogue API   | http://localhost:7001      |
| Order Service API    | http://localhost:7002      |
| RabbitMQ Mgmt UI     | http://localhost:15672     |
| RabbitMQ AMQP        | localhost:5672             |

**Swagger UIs**
- Book Catalogue → `http://localhost:7001/swagger`
- Order Service   → `http://localhost:7002/swagger`

**RabbitMQ UI**
- `http://localhost:15672` (user: `guest`, pass: `guest`)

### 2) Run locally (Visual Studio / `dotnet run`)
- Run RabbitMQ via Docker: `docker compose up -d rabbitmq`
- In each API project’s `launchSettings.json`, set:
  ```json
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "RABBITMQ__HOST": "localhost",
    "RABBITMQ__PORT": "5672",
    "RABBITMQ__USER": "guest",
    "RABBITMQ__PASS": "guest"
  }
  ```
- Use different local ports than Docker (e.g., 7101/7102).

---

## Configuration

All services use 12‑factor style env vars (double underscore `__` for nesting):

```yaml
# docker-compose.yml (excerpt)
book-catalogue:
  environment:
    ASPNETCORE_URLS: http://+:8080
    ASPNETCORE_ENVIRONMENT: Development
    RABBITMQ__HOST: rabbitmq
    RABBITMQ__PORT: "5672"
    RABBITMQ__USER: guest
    RABBITMQ__PASS: guest

order-service:
  environment:
    ASPNETCORE_URLS: http://+:8080
    ASPNETCORE_ENVIRONMENT: Development
    RABBITMQ__HOST: rabbitmq
    RABBITMQ__PORT: "5672"
    RABBITMQ__USER: guest
    RABBITMQ__PASS: guest
    BOOKS_BASE_URL: http://book-catalogue:8080
```

> Inside Docker, services connect to `rabbitmq:5672`. From VS, use `localhost:5672`.




## Troubleshooting

- **MassTransit tries `localhost:5672` inside a container**  
  Ensure env vars inside the container:
  ```bash
  docker compose exec book-catalogue printenv | grep RABBITMQ
  docker compose exec book-catalogue getent hosts rabbitmq
  ```

- **Broker connection refused**  
  Check RabbitMQ listeners & logs:
  ```bash
  docker compose exec rabbitmq rabbitmq-diagnostics listeners
  docker compose logs --tail 200 rabbitmq
  ```

- **Swagger “Failed to load API definition”**  
  Make sure you registered the doc you point the UI at, and the service is on the port you’re visiting.

- **Recreate stack cleanly**
  ```bash
  docker compose down --remove-orphans
  docker compose up -d --build
  ```

---

## Useful Docker commands

```bash
# start / stop / restart
docker compose up -d
docker compose stop
docker compose start
docker compose restart

# down (remove containers & network)
docker compose down
docker compose down -v  # also removes named volumes

# rebuild / recreate
docker compose build --no-cache
docker compose up -d --force-recreate

# status & logs
docker compose ps
docker compose logs -f
docker compose logs -f order-service
```


## License

MIT
