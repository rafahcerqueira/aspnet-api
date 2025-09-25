# Minimal API ■
API simples desenvolvida com **ASP.NET**, utilizando **Entity Framework Core** com **MySQL** O projeto também inclui configuração para **Swagger**, suporte a **MSTest** para testes e autenticação com **JWT**

## 📂 Estrutura do Projeto
```
📦 minimal-api
├── 📂 Api
│   ├── minimal-api.csproj
│   ├── Program.cs
│   └── ...
├── 📂 MSTest
│   ├── MSTest.csproj
│   └── ...
├── 📄 docker-compose.yml
└── 📄 minimal-api.sln
```
---

## ⚙️ Pré-requisitos
Antes de rodar o projeto, garanta que você tem instalado em sua máquina:
- [Docker](https://www.docker.com/)
- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) (ou versão compatível)
- [Visual Studio Code](https://code.visualstudio.com/)

## 🐳 Configuração do Banco de Dados (MySQL via Docker)
O projeto já possui um `docker-compose.yml` configurado para subir um banco **MySQL**.

### 🔑 Variáveis de ambiente necessárias:
Crie um arquivo `.env` na raiz do projeto com o seguinte conteúdo (ajuste conforme necessário):
```
MYSQL_ROOT_PASSWORD=root
MYSQL_DATABASE=MinimalApiDb
MYSQL_USER=admin
MYSQL_PASSWORD=admin123
```

### ▶️ Subindo o container:
```
docker-compose up -d
```
Isso irá:
- Criar um container MySQL acessível na porta `3306`
- Criar o banco definido em `${MYSQL_DATABASE}`
- Executar o script `script.sql` para inicializar a base
---

## ■ Rodando a API
Na raiz do projeto, execute:
```
dotnet build
dotnet run --project Api/minimal-api.csproj
```
A API ficará disponível em:
```
http://localhost:5000
```

### 📖 Swagger
A documentação interativa pode ser acessada em:
```
http://localhost:5000/swagger
```
---

## 🔑 Autenticação JWT
A API utiliza **JWT** para autenticação.
No Swagger, clique em **Authorize** e insira o token no formato:
```
Bearer seu_token_jwt
```
---

## 🧪 Executando os Testes (MSTest)
Os testes estão no projeto `MSTest/`.
Para rodar os testes:
```
dotnet test MSTest/MSTest.csproj
```
Se quiser rodar todos os testes da solução:
```
dotnet test
```
---

## 📌 Comandos Úteis
- **Restaurar dependências**:
 ```
 dotnet restore
 ```
- **Compilar a solução**:
 ```
 dotnet build
 ```
- **Rodar a API em modo desenvolvimento**:
 ```
 dotnet watch run --project Api/minimal-api.csproj
 ```
- **Parar containers Docker**:
 ```
 docker-compose down
 ```
---

## 📜 Licença
Este projeto é livre para estudo e aprimoramento.