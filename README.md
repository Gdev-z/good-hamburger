# 🍔 Good Hamburger API

Sistema de registro de pedidos para a lanchonete **Good Hamburger**, construído com **.NET 8 / ASP.NET Core** seguindo os princípios da **Clean Architecture**.

---

## Estrutura do Projeto

```
GoodHamburger/
├── GoodHamburger.Domain/          # Entidades, Enums, Exceções de domínio
├── GoodHamburger.Application/     # DTOs, Interfaces, Services (lógica de negócio)
├── GoodHamburger.Infrastructure/  # EF Core InMemory, Repositories
├── GoodHamburger.API/             # Controllers, Middleware, Program.cs
├── GoodHamburger.Web/             # Frontend Blazor WebAssembly
└── GoodHamburger.Tests/           # Testes unitários (xUnit + Moq + FluentAssertions)
```

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Como executar

### 1. API REST

```bash
cd GoodHamburger.API
dotnet run
```

A API sobe em `http://localhost:5000`.  
Swagger UI disponível em: `http://localhost:5000/swagger`

### 2. Frontend Blazor (opcional)

```bash
cd GoodHamburger.Web
dotnet run
```

Acesse `http://localhost:5001` (ou a porta exibida no terminal).  
> O Blazor faz chamadas para `http://localhost:5000` — certifique-se de que a API já está rodando.

### 3. Testes unitários

```bash
cd GoodHamburger.Tests
dotnet test --logger "console;verbosity=normal"
```

**11 testes, todos passando.**

---

## Endpoints da API

| Método | Rota              | Descrição                         |
|--------|-------------------|-----------------------------------|
| GET    | `/api/menu`       | Retorna o cardápio completo       |
| GET    | `/api/orders`     | Lista todos os pedidos            |
| GET    | `/api/orders/{id}`| Consulta pedido por ID            |
| POST   | `/api/orders`     | Cria um novo pedido               |
| PUT    | `/api/orders/{id}`| Atualiza itens de um pedido       |
| DELETE | `/api/orders/{id}`| Remove um pedido                  |

### Exemplo — Criar pedido

```json
POST /api/orders
{
  "menuItemIds": [1, 4, 5]
}
```

**Cardápio (IDs fixos no seed):**

| ID | Nome          | Preço | Categoria      |
|----|---------------|-------|----------------|
| 1  | X Burger      | 5,00  | Sanduíche      |
| 2  | X Egg         | 4,50  | Sanduíche      |
| 3  | X Bacon       | 7,00  | Sanduíche      |
| 4  | Batata Frita  | 2,00  | Acompanhamento |
| 5  | Refrigerante  | 2,50  | Bebida         |

---

## Regras de Negócio

### Restrições por pedido

- Máximo **1 sanduíche**, **1 batata frita** e **1 refrigerante** por pedido.
- Itens duplicados retornam HTTP 422 com mensagem clara.

### Descontos automáticos

| Combinação                         | Desconto |
|------------------------------------|----------|
| Sanduíche + Batata + Refrigerante  | 20%      |
| Sanduíche + Refrigerante           | 15%      |
| Sanduíche + Batata                 | 10%      |
| Somente sanduíche                  | 0%       |

### Campos calculados e persistidos

- **Subtotal**: soma dos preços dos itens.
- **Discount**: valor monetário do desconto.
- **Total**: `subtotal - discount`.

---

## Decisões de Arquitetura

### Clean Architecture

Cada camada tem responsabilidade única e dependências sempre apontam para dentro:

```
API → Application → Domain
Infrastructure → Application → Domain
```

- **Domain**: zero dependências externas. Entidades puras (`Order`, `MenuItem`, `OrderItem`), enums e exceções de domínio.
- **Application**: contém as interfaces dos repositórios e os serviços (`OrderService`, `MenuService`, `DiscountService`). Sem referência a EF Core ou frameworks HTTP.
- **Infrastructure**: implementa os repositórios usando EF Core InMemory. Depende apenas de Application.
- **API**: cola tudo com DI container, expõe controllers REST e o middleware de erros.

### Isolamento do cálculo de descontos

`DiscountService` é uma classe estática pura (`static`) sem dependências injetadas — recebe uma lista de `MenuItem` e devolve a tupla `(subtotal, discount, total)`. Isso facilita os testes unitários e garante que a lógica de negócio não vaza para os repositórios ou controllers.

### Tratamento global de erros

`ExceptionMiddleware` intercepta todas as exceções antes de chegarem ao cliente:

| Exceção             | Status HTTP | Quando                                    |
|---------------------|-------------|-------------------------------------------|
| `NotFoundException` | 404         | Pedido/recurso não encontrado             |
| `DomainException`   | 422         | Regra de negócio violada (duplicatas etc.)|
| `Exception` (geral) | 500         | Erros inesperados                         |

### Banco de dados em memória

EF Core InMemory foi escolhido para eliminar dependências externas. O seed é feito via `OnModelCreating` e `Database.EnsureCreated()` na inicialização.

---

## O que ficou de fora

- **Autenticação/Autorização**: não está no escopo do desafio.
- **Paginação** na listagem de pedidos.
- **Banco de dados persistente**: trivial trocar `UseInMemoryDatabase` por `UseSqlite` ou `UseSqlServer` sem mudar nada além do `Program.cs`.
- **Testes de integração**: cobertura focou nas regras de negócio conforme solicitado.
