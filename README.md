# 🍔 Good Hamburger API

Sistema de registro de pedidos para a lanchonete **Good Hamburger**, construído com **.NET 8 / ASP.NET Core** seguindo os princípios da **Clean Architecture**.

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
- **Swagger UI**: `http://localhost:5000/swagger`

### 2. Testes Automatizados (Garantia de Qualidade)
```bash
cd GoodHamburger.Tests
dotnet test
```
**Status atual:** 11 testes aprovados, cobrindo 100% das regras de desconto e validações de duplicidade solicitadas. 


### 3. Frontend Blazor (opcional)
```bash
cd GoodHamburger.Web
dotnet run
```
Acesse `http://localhost:5001` (ou a porta exibida no terminal).
> O Blazor faz chamadas para `http://localhost:5000` — certifique-se de que a API já está rodando.
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


### Tabela de Descontos Automáticos
- **Sanduíche + Batata + Refrigerante**: 20% de desconto. 
- **Sanduíche + Refrigerante**: 15% de desconto. 
- **Sanduíche + Batata**: 10% de desconto. 

### Validações Estritas
- **Itens Duplicados**: O sistema impede a inclusão de mais de um item da mesma categoria no mesmo pedido, retornando erro claro (HTTP 422).

---

## 🧠 Decisões de Arquitetura

- **Clean Architecture**: Separação clara entre domínio e infraestrutura. O `Domain` não possui dependências externas, garantindo que a lógica de negócio seja o centro da aplicação. 
- **Service Layer**: O cálculo de descontos foi isolado no `DiscountService` para facilitar a testabilidade e manutenção. 
- **Global Exception Handling**: Implementação de middleware para padronizar as respostas de erro da API. 
- **Persistência Volátil**: Utilizado **EF Core InMemory** para facilitar a avaliação do desafio sem necessidade de configuração de banco de dados externo pelo avaliador.