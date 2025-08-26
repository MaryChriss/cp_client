# ClienteHub 

Solução em .NET com arquitetura de **microsserviços** para gestão de clientes, integrando:

* **Customer.Api** (CRUD + Oracle via EF Core)  
* **LookupProxy.Api** (proxy p/ **BrasilAPI** com resiliência **Polly**)  
* **Client.Web** (MVC que consome as duas APIs)  
* **Libraries**: ClienteHub.Domain, ClienteHub.Application, ClienteHub.Infrastructure (com **Migrations**)  

---

## 📦 Estrutura dos projetos
```

ClienteHub
├─ Customer.Api                 # WebAPI (Oracle + EF Core)
├─ LookupProxy.Api              # WebAPI (Proxy p/ BrasilAPI + Polly)
├─ Client.Web                   # MVC (front)
├─ ClienteHub.Domain            # Entidades + Interfaces (SRP/DIP)
├─ ClienteHub.Application       # Serviços/DTOs (regras de negócio)
└─ ClienteHub.Infrastructure    # EF Core (DbContext, Repositórios, Migrations)

````

---

## ✅ Pré-requisitos
* **.NET SDK** 8 ou 9 (projeto usa 9)  
* **Oracle Database XE 21c**  
* **Host/Port/Service** usados aqui: `localhost:1522/XEPDB1`  
* Serviços ativos: **OracleServiceXE** e **OracleOraDB21Home1TNSListener**  
* **dotnet-ef** (ferramenta de migrations)  

```powershell
dotnet tool update --global dotnet-ef
````

> Se for usar outro banco, adapte o provider e as strings de conexão.

---

## 🔧 Configuração de conexão (Oracle)

### 1) Criar usuário de aplicação (via SQL Developer como SYSTEM no XEPDB1)

```sql
CREATE USER CLIENTEHUB IDENTIFIED BY "SenhaForte123"
  DEFAULT TABLESPACE USERS
  TEMPORARY TABLESPACE TEMP
  QUOTA UNLIMITED ON USERS;

GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE, CREATE TRIGGER TO CLIENTEHUB;
```

### 2) Customer.Api/appsettings.json

```json
{
  "ConnectionStrings": {
    "Oracle": "User Id=CLIENTEHUB;Password=SenhaForte123;Data Source=localhost:1522/XEPDB1;"
  },
  "AllowedHosts": "*"
}
```

---

## 🗃️ Migrations (na **library** ClienteHub.Infrastructure)

> Rode na **raiz da solução**. Defina a variável (usada pela `DesignTimeDbContextFactory`):

```powershell
$env:ORACLE_CS="User Id=CLIENTEHUB;Password=SenhaForte123;Data Source=localhost:1522/XEPDB1;"
```

Crie a migration inicial (se ainda não existir):

```powershell
dotnet ef migrations add InitialCreate --project ClienteHub.Infrastructure/ClienteHub.Infrastructure.csproj --startup-project Customer.Api/Customer.Api.csproj --output-dir Data/Migrations
```

Atualize o banco:

```powershell
dotnet ef database update --project ClienteHub.Infrastructure/ClienteHub.Infrastructure.csproj --startup-project Customer.Api/Customer.Api.csproj
```

> Tabelas criadas: **TB\_CUSTOMERS**, **TB\_ADDRESSES** (schema CLIENTEHUB).

---

## ▶️ Como rodar localmente

Em 3 terminais (ou Start sem depuração no VS):

```powershell
# 1) API Oracle (CRUD)
dotnet run --project Customer.Api/Customer.Api.csproj --urls http://localhost:5101

# 2) API Proxy (BrasilAPI)
dotnet run --project LookupProxy.Api/LookupProxy.Api.csproj --urls http://localhost:5102

# 3) MVC
dotnet run --project Client.Web/Client.Web.csproj --urls http://localhost:5103
```

### URLs úteis

* **Customer.Api Swagger**: [http://localhost:5101/swagger](http://localhost:5101/swagger)
* **LookupProxy.Api Swagger**: [http://localhost:5102/swagger](http://localhost:5102/swagger)
* **Client.Web (MVC)**: [http://localhost:5103/](http://localhost:5103/)

> No **Client.Web/appsettings.json**:

```json
{
  "Services": {
    "CustomerApi": "http://localhost:5101/",
    "LookupApi": "http://localhost:5102/"
  }
}
```

---

## 🔌 Endpoints principais

### Customer.Api (CRUD)

**Base URL:** `http://localhost:5101`

* **GET /api/customers**
  **200 OK** (exemplo):

```json
[
  {
    "id": "3e0a3f0b-8f5c-4d5a-8b7e-bf2a2d5b1234",
    "name": "Maria Silva",
    "email": "maria@exemplo.com",
    "cnpj": "19131243000197",
    "cep": "01310930",
    "street": "Av. Paulista",
    "neighborhood": "Bela Vista",
    "city": "São Paulo",
    "state": "SP",
    "number": "1000",
    "complement": "Conj. 101"
  }
]
```

* **GET /api/customers/{id}** → **200** ou **404**
* **POST /api/customers**
  **Request**:

```json
{
  "name": "Empresa Teste",
  "email": "teste@empresa.com",
  "cnpj": "19131243000197",
  "cep": "01310930",
  "street": "Av. Paulista",
  "neighborhood": "Bela Vista",
  "city": "São Paulo",
  "state": "SP",
  "number": "1000",
  "complement": "Conj. 101"
}
```

**Response**: **201 Created** (Location com id)

* **PUT /api/customers/{id}** → **204 No Content**
* **DELETE /api/customers/{id}** → **204 No Content**

---

### LookupProxy.Api (BrasilAPI + Polly)

**Base URL:** `http://localhost:5102`

* **GET /cep/{cep}**
  **Exemplo:** `/cep/01310923`
  **200 OK**:

```json
{
  "cep": "01310923",
  "state": "SP",
  "city": "São Paulo",
  "neighborhood": "Bela Vista",
  "street": "Avenida Paulista",
  "service": "brasilapi"
}
```

* **GET /cnpj/{cnpj}**
  **Exemplo:** `/cnpj/19131243000197`
  **200 OK**:

```json
{
  "cnpj": "19131243000197",
  "razao_social": "ACME LTDA",
  "nome_fantasia": "ACME",
  "...": "..."
}
```

**Resiliência (Polly):** retry exponencial (3x), timeout (5s) e circuit-breaker para erros transitórios; retorna `ProblemDetails` quando a BrasilAPI falha.

---

## 🧠 SOLID aplicado (breve)

* **SRP (Single Responsibility Principle)**

  * CustomerService: orquestra regras de cliente.
  * EfCustomerRepository: acesso a dados (EF Core/Oracle).
  * CustomersController: expõe HTTP/REST.

* **OCP (Open/Closed Principle)**

  * CustomerService depende de `ICustomerRepository`.
  * Trocar Oracle/EF por outro repositório não exige alterar CustomerService.

* **DIP (Dependency Inversion Principle)**

  * Application depende de **abstrações** do Domain (`ICustomerRepository`), não de implementações concretas.
  * Inversão concretizada via DI no `Customer.Api/Program.cs`.

*(Clean Code: nomeação clara, classes coesas, baixo acoplamento, mínima lógica em controllers, DTOs explícitos.)*

---

## 🧩 System Design (Mermaid)

> Coloque este bloco no README — o GitHub renderiza Mermaid.

```mermaid
flowchart LR
    subgraph MVC [Client.Web (MVC)]
      A[Views/Controllers] -->|HTTP| CAPI[(Customer.Api)]
      A -->|HTTP| LAPI[(LookupProxy.Api)]
    end

    subgraph CustomerApi [Customer.Api]
      CAPI -->|DI: ICustomerRepository| SVC[CustomerService]
      SVC --> REPO[EfCustomerRepository]
      REPO --> DB[(Oracle XE 21c<br/>XEPDB1, schema CLIENTEHUB)]
    end

    subgraph LookupApi [LookupProxy.Api]
      LAPI -->|HttpClient + Polly| BAPI[(BrasilAPI:<br/>/cep /cnpj)]
    end
```

---

## 🧪 Teste rápido (curl)

```bash
# CEP via proxy
curl http://localhost:5102/cep/01310923

# CNPJ via proxy
curl http://localhost:5102/cnpj/19131243000197

# CRUD
curl http://localhost:5101/api/customers
curl -X POST http://localhost:5101/api/customers -H "Content-Type: application/json" -d @- <<EOF
{
  "name":"Empresa Teste",
  "email":"teste@empresa.com",
  "cnpj":"19131243000197",
  "cep":"01310930",
  "street":"Av. Paulista",
  "neighborhood":"Bela Vista",
  "city":"São Paulo",
  "state":"SP",
  "number":"1000",
  "complement":"Conj. 101"
}
EOF
```

---

## 🛠️ Dicas & Troubleshooting

* **ORA-12541 (no listener):** inicie `OracleOraDB21Home1TNSListener` e `OracleServiceXE` (Services.msc) e confirme `lsnrctl status`.
* **ORA-00942 (table not found):** faltou rodar migrations no **usuário correto** (CLIENTEHUB) do **XEPDB1**.
* **EF versão conflitante:** alinhe pacotes EF para **9.0.8** nos projetos API/Infrastructure.
* **MVC quebrando ao chamar API:** confira `Client.Web/appsettings.json` → seção `Services` com URLs corretas (`/` no final).

---

## 🔒 Segurança

* **NÃO** versionar credenciais reais.
* Em dev, prefira **User Secrets** ou variável de ambiente (`ORACLE_CS`).
* `appsettings.json` aqui contém apenas exemplos.


Quer que eu monte também uma **versão enxuta**, só com os pontos essenciais (descrição, pré-requisitos, como rodar e endpoints), para você usar como alternativa rápida no README?
```
