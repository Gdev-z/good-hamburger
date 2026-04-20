# 🍔 Good Hamburger API

[cite_start]Sistema de registro de pedidos para a lanchonete **Good Hamburger**, construído com **.NET 8 / ASP.NET Core** seguindo os princípios da **Clean Architecture**. [cite: 20]

---

## 🏗️ Estrutura do Projeto

```
GoodHamburger/
├── GoodHamburger.Domain/          # Entidades, Enums, Exceções de domínio
├── GoodHamburger.Application/     # DTOs, Interfaces, Services (lógica de negócio)
├── GoodHamburger.Infrastructure/  # EF Core InMemory, Repositories
├── GoodHamburger.API/             # Controllers, Middleware, Program.cs
├── GoodHamburger.Web/             # Frontend Blazor WebAssembly (Diferencial)
└── GoodHamburger.Tests/           # Testes unitários (xUnit + Moq + FluentAssertions)
```

---

## 🚀 Como executar

### 1. API REST
```bash
cd GoodHamburger.API
dotnet run
```
- [cite_start]**Swagger UI**: `http://localhost:5000/swagger` [cite: 24]

### 2. Testes Automatizados (Garantia de Qualidade)
```bash
cd GoodHamburger.Tests
dotnet test
```
[cite_start]**Status atual:** 11 testes aprovados, cobrindo 100% das regras de desconto e validações de duplicidade solicitadas. [cite: 27]

---

## 📋 Regras de Negócio Implementadas

### Cardápio e Preços
| ID | Nome | Preço | Categoria |
|---|---|---|---|
| 1 | X Burger | R$ 5,00 | Sanduíche |
| 2 | X Egg | R$ 4,50 | Sanduíche |
| 3 | X Bacon | R$ 7,00 | Sanduíche |
| 4 | Batata Frita | R$ 2,00 | Acompanhamento |
| 5 | Refrigerante | R$ 2,50 | Bebida |
[cite_start][cite: 8, 9, 10, 13]

### Tabela de Descontos Automáticos
- [cite_start]**Sanduíche + Batata + Refrigerante**: 20% de desconto. [cite: 12, 14]
- [cite_start]**Sanduíche + Refrigerante**: 15% de desconto. [cite: 15]
- [cite_start]**Sanduíche + Batata**: 10% de desconto. [cite: 16]

### Validações Estritas
- [cite_start]**Itens Duplicados**: O sistema impede a inclusão de mais de um item da mesma categoria no mesmo pedido, retornando erro claro (HTTP 422). [cite: 17, 18]

---

## 🧠 Decisões de Arquitetura

- **Clean Architecture**: Separação clara entre domínio e infraestrutura. [cite_start]O `Domain` não possui dependências externas, garantindo que a lógica de negócio seja o centro da aplicação. [cite: 5]
- [cite_start]**Service Layer**: O cálculo de descontos foi isolado no `DiscountService` para facilitar a testabilidade e manutenção. [cite: 22]
- [cite_start]**Global Exception Handling**: Implementação de middleware para padronizar as respostas de erro da API. [cite: 23]
- **Persistência Volátil**: Utilizado **EF Core InMemory** para facilitar a avaliação do desafio sem necessidade de configuração de banco de dados externo pelo avaliador.