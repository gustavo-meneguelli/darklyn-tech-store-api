# Darklyn Tech Store API

Este repositório contém a API Backend de um e-commerce de produtos tecnológicos.

O objetivo principal deste projeto não é apenas criar um CRUD, mas sim aplicar Engenharia de Software e boas práticas de mercado, simulando um ambiente real de desenvolvimento corporativo utilizando o ecossistema .NET mais recente.

Swagger (Live Demo): https://darklyn-api.onrender.com/swagger
Front-end (Visual): https://github.com/gustavo-meneguelli/darklyn-tech-store-web

---

## Tecnologias e Práticas

O projeto foi construído focando em desacoplamento, testabilidade e performance.

- Framework: .NET 10 (C#)
- Arquitetura: Clean Architecture (Domain, Application, Infrastructure, API)
- Banco de Dados: PostgreSQL
- ORM: Entity Framework Core (Code First)
- Autenticação: JWT (JSON Web Token)
- Validação: FluentValidation (Fail Fast Strategy)
- Mapeamento: AutoMapper
- Containerização: Docker & Docker Hub
- Testes: xUnit (Unitários e de Integração)

## Estrutura do Projeto

A solução segue a separação de responsabilidades estrita:

- Domain: Entidades, Enums e Regras de Negócio "Puras". Não depende de nenhuma outra camada.
- Application: Casos de uso (Services), DTOs, Interfaces e Validações.
- Infrastructure: Implementação de acesso a dados (Repositories), Contexto do Banco e Configurações externas.
- Api: Controllers, Middlewares e Injeção de Dependência (IoC).

## Funcionalidades Principais

- Gestão de Produtos: CRUD completo com relacionamento de categorias.
- Integridade de Dados: Restrições de chave estrangeira (Delete Restrict) para evitar registros órfãos.
- Segurança: Hash de senhas e proteção de rotas via Role-based Authorization (Admin/User).
- Auditoria: Implementação de Soft Delete e rastreamento de data de criação/edição.

## Como Rodar Localmente

### Pré-requisitos
- .NET SDK 10.0+
- Docker (Recomendado para o Banco de Dados)
- PostgreSQL

### Passo a passo

1. Clone o repositório:
   git clone https://github.com/gustavo-meneguelli/darklyn-tech-store-api.git

2. Configure o Banco de Dados:
   Atualize a string de conexão no appsettings.Development.json ou utilize o user-secrets para maior segurança.

3. Execute as Migrations:
   dotnet ef database update

4. Rode a API:
   dotnet run --project src/Api

   O Swagger estará disponível em http://localhost:5000/swagger (ou na porta configurada).

## Testes

O projeto possui uma suíte de testes automatizados para garantir a estabilidade das regras de negócio e da integração.

Para executar os testes:
dotnet test

---
Desenvolvido por Gustavo Meneguelli.