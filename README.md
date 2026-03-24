# Sistema de Controle de Despesas Residenciais

Um sistema completo de controle de despesas residenciais com backend em .NET Core, frontend em React TypeScript e banco de dados SQL Server.

## Funcionalidades

### Backend (.NET Web API)
- Gerenciamento de Pessoas (CRUD com exclusão em cascata)
- Gerenciamento de Categorias (CRUD)
- Gerenciamento de Transações com regras de negócio:
  - Menores de idade (idade < 18) só podem criar transações de despesa
  - Validação de compatibilidade da categoria com base na finalidade
- Relatórios de Resumo:
  - Totais por Pessoa com resumo geral
  - Totais por Categoria com resumo geral (opcional)
- Documentação da API com Swagger

### Frontend (React + TypeScript)
- Layout responsivo com navegação
- Gerenciamento de Pessoas (criar, editar, excluir com confirmação)
- Gerenciamento de Categorias (criar, lista somente leitura)
- Gerenciamento de Transações (criar com filtragem dinâmica)
- Relatórios com totais resumidos
- Validação de formulários e tratamento de erros

## Como Executar a Aplicação

### Pré-requisitos

- **Docker** e **Docker Compose** instalados
- **Git** para clonar o repositório
- Opcional: **Node.js 18+** e **.NET 7 SDK** para desenvolvimento local

### 🚀 Início Rápido com Docker (Recomendado)

1. **Clone o repositório:**
   ```bash
   git clone <url-do-repositorio>
   cd controle-gastos-residenciais
   ```

2. **Execute a aplicação:**
   ```bash
   # Na raiz do projeto (onde está o docker-compose.yml)
   docker-compose up -d --build
   ```

3. **Aguarde a inicialização:**
   - O SQL Server pode levar alguns segundos para iniciar
   - O backend aplicará automaticamente as migrações do banco de dados

4. **Acesse a aplicação:**
   - **API Base:** http://localhost:5000
   - **Documentação Swagger:** http://localhost:5000/swagger
   - **Banco de dados:** localhost:1433 (SQL Server)

### 🛠️ Desenvolvimento Local (Opcional)

#### Backend (.NET)
```bash
cd backend

# Restaure as dependências
dotnet restore

# Execute as migrações (certifique-se de que o SQL Server está rodando)
dotnet ef database update

# Execute a aplicação
dotnet run --project ControleGastosResidenciais.API
```

#### Frontend (React)
```bash
cd frontend

# Instale as dependências
npm install

# Execute em modo desenvolvimento
npm start
```

### 📊 Endpoints da API

A API estará disponível em `http://localhost:5000` com os seguintes endpoints principais:

#### Pessoas
- `GET /api/pessoas` - Listar todas as pessoas
- `POST /api/pessoas` - Criar nova pessoa
- `PUT /api/pessoas/{id}` - Atualizar pessoa
- `DELETE /api/pessoas/{id}` - Excluir pessoa

#### Categorias
- `GET /api/categorias` - Listar todas as categorias
- `POST /api/categorias` - Criar nova categoria
- `PUT /api/categorias/{id}` - Atualizar categoria
- `DELETE /api/categorias/{id}` - Excluir categoria

#### Transações
- `GET /api/transacoes` - Listar todas as transações
- `POST /api/transacoes` - Criar nova transação
- `PUT /api/transacoes/{id}` - Atualizar transação
- `DELETE /api/transacoes/{id}` - Excluir transação

#### Relatórios
- `GET /api/relatorios/resumo-pessoas` - Relatório por pessoa
- `GET /api/relatorios/resumo-categorias` - Relatório por categoria

### Postman Collection

Para facilitar os testes da API, incluímos uma coleção completa do Postman com todos os endpoints documentados:

**Arquivo:** `ControleGastosResidenciais.postman_collection.json`

**Como usar:**
1. Importe o arquivo na sua aplicação Postman
2. Configure a variável `baseUrl` para `http://localhost:5000`
3. Execute os requests na ordem sugerida (criar pessoas/categorias antes das transações)

A coleção inclui exemplos de requests com bodies JSON válidos e documentação detalhada de cada endpoint.

### Banco de Dados

- **Servidor:** SQL Server 2022 Express
- **Porta:** 1433
- **Banco:** ControleGastosResidenciaisDB
- **Usuário:** sa
- **Senha:** admin@123

**Nota:** As migrações são aplicadas automaticamente quando o container inicia.

### 🛑 Como Parar a Aplicação

```bash
# Pare e remova os containers
docker-compose down

# Pare os containers mantendo os volumes (dados preservados)
docker-compose down -v  # Remove também os volumes
```

### 🔍 Verificação de Status

```bash
# Verificar status dos containers
docker-compose ps

# Ver logs do backend
docker-compose logs backend

# Ver logs do banco de dados
docker-compose logs sql-server
```

### 📝 Notas Importantes

- A aplicação utiliza **Swagger** para documentação da API, acessível em desenvolvimento e produção (EnableSwaggerInProduction=true), somente para demonstração
- O banco de dados persiste dados entre reinicializações através de volumes Docker
- Para desenvolvimento, considere usar o modo Development para recursos adicionais de debug