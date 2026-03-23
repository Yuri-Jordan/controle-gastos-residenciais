#!/bin/bash

echo "Criando Estrutura do Backend de Controle de Gastos..."

# Criar diretório principal do backend
mkdir -p backend
cd backend

# Criar arquivo de solução
dotnet new sln -n ControleGastosResidenciais

# Criar projetos
dotnet new classlib -n ControleGastosResidenciais.Core -f net9.0
dotnet new classlib -n ControleGastosResidenciais.Infraestrutura -f net9.0
dotnet new webapi -n ControleGastosResidenciais.API -f net9.0
dotnet new xunit -n ControleGastosResidenciais.Testes -f net9.0

# Adicionar à solução
dotnet sln add ControleGastosResidenciais.Core/ControleGastosResidenciais.Core.csproj
dotnet sln add ControleGastosResidenciais.Infraestrutura/ControleGastosResidenciais.Infraestrutura.csproj
dotnet sln add ControleGastosResidenciais.API/ControleGastosResidenciais.API.csproj
dotnet sln add ControleGastosResidenciais.Testes/ControleGastosResidenciais.Testes.csproj

# Adicionar referências
dotnet add ControleGastosResidenciais.Infraestrutura/ControleGastosResidenciais.Infraestrutura.csproj reference ControleGastosResidenciais.Core/ControleGastosResidenciais.Core.csproj
dotnet add ControleGastosResidenciais.API/ControleGastosResidenciais.API.csproj reference ControleGastosResidenciais.Core/ControleGastosResidenciais.Core.csproj
dotnet add ControleGastosResidenciais.API/ControleGastosResidenciais.API.csproj reference ControleGastosResidenciais.Infraestrutura/ControleGastosResidenciais.Infraestrutura.csproj
dotnet add ControleGastosResidenciais.Testes/ControleGastosResidenciais.Testes.csproj reference ControleGastosResidenciais.Core/ControleGastosResidenciais.Core.csproj
dotnet add ControleGastosResidenciais.Testes/ControleGastosResidenciais.Testes.csproj reference ControleGastosResidenciais.Infraestrutura/ControleGastosResidenciais.Infraestrutura.csproj
dotnet add ControleGastosResidenciais.Testes/ControleGastosResidenciais.Testes.csproj reference ControleGastosResidenciais.API/ControleGastosResidenciais.API.csproj

# Adicionar pacotes ao Core
cd ControleGastosResidenciais.Core
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
cd ..

# Adicionar pacotes à Infraestrutura
cd ControleGastosResidenciais.Infraestrutura
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package Microsoft.Extensions.Configuration.Json
cd ..

# Adicionar pacotes à API
cd ControleGastosResidenciais.API
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.AspNetCore.Cors
dotnet add package Swashbuckle.AspNetCore
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package Microsoft.EntityFrameworkCore.Tools
cd ..

# Adicionar pacotes aos Testes
cd ControleGastosResidenciais.Testes
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package Moq
dotnet add package FluentAssertions
dotnet add package Microsoft.AspNetCore.Mvc.Testing
cd ..

# Criar estrutura de pastas
mkdir -p ControleGastosResidenciais.Core/Entities
mkdir -p ControleGastosResidenciais.Core/Enums
mkdir -p ControleGastosResidenciais.Core/Interfaces
mkdir -p ControleGastosResidenciais.Core/DTOs
mkdir -p ControleGastosResidenciais.Core/Exceptions
mkdir -p ControleGastosResidenciais.Core/Validators

mkdir -p ControleGastosResidenciais.Infraestrutura/Data
mkdir -p ControleGastosResidenciais.Infraestrutura/Repositories
mkdir -p ControleGastosResidenciais.Infraestrutura/Services
mkdir -p ControleGastosResidenciais.Infraestrutura/Mappings
mkdir -p ControleGastosResidenciais.Infraestrutura/Migrations

mkdir -p ControleGastosResidenciais.API/Controllers
mkdir -p ControleGastosResidenciais.API/Middleware
mkdir -p ControleGastosResidenciais.API/Extensions
mkdir -p ControleGastosResidenciais.API/Filters

mkdir -p ControleGastosResidenciais.Testes/UnitTests
mkdir -p ControleGastosResidenciais.Testes/IntegrationTests
mkdir -p ControleGastosResidenciais.Testes/Fixtures

echo "Estrutura do backend criada com sucesso!"
echo ""
echo "Para verificar a estrutura, execute:"
echo "  cd backend"
echo "  dotnet build"