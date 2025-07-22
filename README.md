# ✈️ Route API (.NET 8)

API para encontrar a rota de menor custo entre dois aeroportos, considerando conexões intermediárias. Desenvolvida como parte de um desafio técnico, utilizando:

- .NET 8
- EF Core (InMemory Database)
- Clean Architecture
- Testes com xUnit

---

## 📦 Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/) (opcional, para execução via container)

---

## 🚀 Executando o Projeto

### 🔧 Via Terminal (sem Docker)

1. Clone o repositório:
   ```bash
   git clone https://github.com/vgoncalez/route-search-api.git
   cd route-search-api
   ```

2. Restaure os pacotes:
   ```bash
   dotnet restore
   ```

3. Rode a aplicação:
   ```bash
   dotnet run --project src/RouteSearchApi
   ```

4. Acesse a documentação Swagger:
   ```
   http://localhost:58131/swagger
   ```

---

### 🐳 Via Docker

1. Compile e suba o container:
   ```bash
   docker-compose up --build
   ```

2. A API estará disponível em:
   ```
   http://localhost:8080/swagger
   ```

---

## 🧪 Executando os Testes

```bash
dotnet test
```

---

## 🛠️ Funcionalidades

- Registrar rotas (Origem, Destino, Valor)
- Calcular a rota mais barata entre dois pontos, considerando todas as conexões possíveis

---

## 📘 Exemplo de Dados Utilizados

Essas rotas são populadas no banco de dados em memória para fins de teste:

| Origem | Destino | Valor |
|--------|---------|--------|
| GRU    | BRC     | 10     |
| BRC    | SCL     | 5      |
| GRU    | CDG     | 75     |
| GRU    | SCL     | 20     |
| GRU    | ORL     | 56     |
| ORL    | CDG     | 5      |
| SCL    | ORL     | 20     |

---

## 🧮 Exemplo de Resultado

Consulta de melhor rota entre **GRU** e **CDG**:

```
GET /api/route/best?origem=GRU&destino=CDG
```

Resposta:
```json
"GRU - BRC - SCL - ORL - CDG ao custo de $40"
```
---

## ✅ Tecnologias Utilizadas

- ASP.NET Core 8
- Entity Framework Core (InMemory)
- MediatR (opcional, se quiser aplicar CQRS)
- xUnit
- Swagger (Swashbuckle)