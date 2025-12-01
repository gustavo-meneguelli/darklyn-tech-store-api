# Product Management API - Estudo

> **Nota:** Este é um projeto puramente educacional. Não sou um programador profissional (ainda), sou um estudante explorando o ecossistema .NET. O código aqui reflete meu aprendizado atual.

## Sobre o Projeto
Estou construindo esta API para estudar conceitos de desenvolvimento Backend com C#. O objetivo é sair do básico e aplicar padrões de mercado, containerização e deploy na nuvem.

O projeto evoluiu de um banco local para uma arquitetura pronta para produção com PostgreSQL.

## 🛠️ Tech Stack & Conceitos Aplicados:
* **.NET 10 Web API**
* **Arquitetura em Camadas** (Domain, Application, Infra, API)
* **Entity Framework Core** com **PostgreSQL**
* **Docker** (Multi-stage build)
* **Autenticação JWT** e RBAC (Admin/Common)
* **Paginação de dados** e Filtros
* **Testes de Integração** (InMemory Database)
* **CI/CD Manual**: Deploy automatizado via Docker Hub e Render.com

## 🚀 Como Rodar (Via Docker - Recomendado)
Como o projeto agora depende de um banco PostgreSQL, a forma mais fácil de rodar é via Docker, pois ele configura o ambiente todo para você.

1. **Clone o repositório** e entre na pasta raiz.
2. **Crie a imagem:**
   ```bash
   docker build -t product-api .
Rode o container:

⚠️ Atenção: Substitua os valores abaixo (senhas e chaves) pelos seus próprios segredos antes de rodar.

Bash

docker run -p 5000:8080 \
  -e "ASPNETCORE_ENVIRONMENT=Development" \
  -e "JwtSettings:SecretKey=DefinaUmaChaveSuperSecretaComMuitosCaracteres123!" \
  -e "AdminSettings:Password=DefinaSuaSenhaDeAdmin" \
  -e "ConnectionStrings:DefaultConnection=Host=SEU_HOST;Port=5432;Database=SEU_DB;Username=SEU_USER;Password=SUA_SENHA" \
  product-api
☁️ Deploy (Produção)
A API está rodando publicamente no Render.com conectada a um banco PostgreSQL gerenciado.

Documentação (Swagger): /swagger

Repositório mantido por Gustavo Meneguelli para fins de estudo.