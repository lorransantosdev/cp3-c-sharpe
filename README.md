# Sistema Funerário — API

Projeto Funerária — API construída em .NET 8 / ASP.NET Core para a disciplina Engenharia de Software (3ESR) — FIAP 2026.

---

## 1. Identificação

| Nome completo            | RM        |
| ------------------------ | --------- |
| Vinicius Gardim          | RM556013  |
| Lorran dos Santos        | RM558982  |

Equipe: **Dupla**.

---

## 2. Serviço funerário escolhido e justificativa

A entrega contempla **dois serviços** expostos na API (escopo dupla):

1. **Plano Funerário (`PlanoFunerario`)** — serviço principal. A regra de negócio cobre análise de crédito completa: cálculo de score, validação de cobertura/carência do plano e ajuste da **mensalidade base** em função do score do cliente. O plano é uma assinatura mensal que garante a cobertura dos serviços assistenciais quando ocorrer um falecimento.
2. **Serviço Assistencial (`ServicoAssistencial`)** — serviço secundário. Cobre contratações pontuais (velório, cremação, traslado). A regra implementa **taxa do pacote premium variável**, ajustada de acordo com o faturamento mensal do cliente PJ (convênios empresariais) ou renda do PF, favorecendo contratantes de maior porte.

`Jazigo` aparece no diagrama de classes e no domínio (especialização de `Servico`), mas não é exposto na API conforme permitido na FAQ do enunciado.

**Por que essas escolhas?** Plano funerário e serviço assistencial são representativos das duas operações típicas de uma funerária moderna (assinatura mensal de cobertura e contratação pontual de serviços), e cada um expõe um motor de decisão diferente — score em plano funerário, faturamento em serviço assistencial —, o que evita "regra de negócio única replicada".

### Regras de negócio extras (escopo dupla/trio)

- **Cálculo de score** em `Funeraria.Domain/Services/AnaliseContratacaoService.cs` — partindo de uma base de 500 pontos, ajusta por tempo de relacionamento, renda/faturamento e idade do titular PF.
- **Mensalidade variável** no plano funerário: ajuste de `±0,50 pp` na mensalidade base conforme faixa de score (≥800, ≥700, ≥600, ≥500, abaixo).
- **Taxa premium variável** no serviço assistencial: redução de até `1,00 pp` na taxa do pacote premium conforme faturamento (≥200k, ≥50k, ≥10k).

---

## 3. Stack

| Camada      | Tecnologia                                               |
| ----------- | -------------------------------------------------------- |
| Runtime     | .NET 8.0                                                 |
| API         | ASP.NET Core Web API                                     |
| ORM         | Entity Framework Core 8 (Oracle.EntityFrameworkCore)     |
| Banco       | Oracle FIAP (`oracle.fiap.com.br:1521/ORCL`)             |
| Mensageria  | Fila em memória (`System.Threading.Channels`) + `BackgroundService` consumer |
| Testes      | xUnit + FluentAssertions + WebApplicationFactory         |

### Estrutura da solução

```
Funeraria.sln
├── src/
│   ├── Funeraria.Domain          # Entidades, enums, validators, serviço de análise
│   ├── Funeraria.Infrastructure  # DbContext, fila in-memory + consumer hospedado
│   └── Funeraria.Api             # Controllers, DTOs, Swagger
└── tests/
    └── Funeraria.Tests           # 26 testes (xUnit + InMemory)
```

> **Observação:** A entrega usa uma fila em memória (`System.Threading.Channels`) + `BackgroundService` para o processamento assíncrono de contratações, em substituição ao RabbitMQ. O contrato `IContratacaoPublisher` permanece, então uma futura troca para RabbitMQ exige apenas substituir a implementação no `DependencyInjection`.

---

## 4. Diagrama de classes

Versão draw.io: [`docs/diagrama-classes.drawio`](docs/diagrama-classes.drawio)
Versão PlantUML (fonte): [`docs/diagrama-classes.puml`](docs/diagrama-classes.puml)

> Para gerar PNG: abra o `.drawio` em [diagrams.net](https://app.diagrams.net) → File → Export As → PNG, salvando em `docs/diagrama-classes.png`.

```
                +----------+               +-----------+
                | Filial   | 1 ──── 0..* | Cliente*   |
                +----------+              | (abstract) |
                                          +-----------+
                                            ▲       ▲
                                            │       │
                                  +----------------+ +-----------------+
                                  | PessoaFisica   | | PessoaJuridica  |
                                  | + Cpf, Nasc... | | + Cnpj, Razão...|
                                  +----------------+ +-----------------+
                                            │ (1)
                                            │
                                       0..* │
                                       +----------------+
                                       | Contratacao    |
                                       | + Status, Score|
                                       +----------------+
                                            │ 0..*
                                            │
                                            │ 1
                                       +----------------+
                                       | Servico*       |
                                       +----------------+
                                            ▲
                          ┌─────────────────┼──────────────────────┐
                          │                 │                      │
              +---------------------+ +---------+ +---------------------+
              | ServicoAssistencial | | Jazigo  | | PlanoFunerario      |
              +---------------------+ +---------+ +---------------------+
```

---

## 5. Como executar

### 5.1. Pré-requisitos

- .NET SDK 8.x ou 9.x (o projeto compila em `net8.0` em ambos)
- Acesso ao Oracle FIAP (`oracle.fiap.com.br:1521/ORCL`) com seu RM
- `dotnet-ef` instalado globalmente: `dotnet tool install --global dotnet-ef`

### 5.2. Configuração

Edite `src/Funeraria.Api/appsettings.json` com sua connection string Oracle:

```json
"ConnectionStrings": {
  "Oracle": "User Id=RM558982;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/ORCL;"
}
```

### 5.3. Migrations

Aplicar o schema no Oracle (na primeira execução):

```bash
dotnet ef database update --project src/Funeraria.Infrastructure --startup-project src/Funeraria.Api
```

Se precisar regenerar a migration (após editar entidades), apague os arquivos de `src/Funeraria.Infrastructure/Migrations/` e rode:

```bash
dotnet ef migrations add InitialCreate --project src/Funeraria.Infrastructure --startup-project src/Funeraria.Api
```

> O `Program.cs` da API também aplica migrations automaticamente quando há connection string configurada e o ambiente é `Development`.

### 5.4. Subir a API

```bash
dotnet run --project src/Funeraria.Api
# → http://localhost:5080/swagger
```

A API sobe a Web API + o `ContratacaoBackgroundConsumer` no mesmo processo. Toda solicitação `POST /api/contratacoes` é gravada como `Pendente`, enfileirada na `InMemoryContratacaoQueue` e processada pelo background service em segundo plano.

---

## 6. Endpoints disponíveis

Todos os endpoints estão documentados via Swagger em `http://localhost:5080/swagger`.

### Filiais

#### `POST /api/filiais`
```json
// request
{ "numero": "0001", "nome": "Filial Centro", "endereco": "Av. Paulista, 1000" }

// 201 Created
{ "id": 1, "numero": "0001", "nome": "Filial Centro", "endereco": "Av. Paulista, 1000" }
```

#### `GET /api/filiais/{id}` → `200 OK` ou `404 Not Found`

### Clientes

#### `POST /api/clientes/pf`
```json
// request
{
  "nome": "João da Silva",
  "email": "joao@example.com",
  "telefone": "11999990000",
  "filialId": 1,
  "cpf": "529.982.247-25",
  "dataNascimento": "1990-01-15",
  "rendaMensal": 8000
}

// 201 Created
{
  "id": 10, "tipo": "PF", "nome": "João da Silva",
  "cpf": "52998224725", "filialId": 1, "rendaMensal": 8000.00, ...
}
```
- `400 Bad Request` se CPF inválido
- `404 Not Found` se filial não existir
- `409 Conflict` se CPF já cadastrado

#### `POST /api/clientes/pj`
```json
// request
{
  "nome": "Convênio XPTO",
  "email": "contato@xpto.com",
  "telefone": "1130000000",
  "filialId": 1,
  "cnpj": "11.222.333/0001-81",
  "razaoSocial": "XPTO Convênios Ltda",
  "faturamentoMensal": 50000
}
```
Mesmas regras de erro do PF, validando CNPJ.

#### `GET /api/clientes/{id}` → retorna PF ou PJ.

### Serviços

#### `POST /api/servicos/plano-funerario`
```json
{
  "nome": "Plano Família",
  "descricao": "Plano de assistência funerária para a família",
  "mensalidadeBase": 0.025,
  "carenciaMaximaMeses": 36,
  "coberturaMaxima": 50000
}
```

#### `POST /api/servicos/servico-assistencial`
```json
{
  "nome": "Velório Premium",
  "descricao": "Velório completo com cerimônia",
  "taxaPacoteBasico": 0.0149,
  "taxaPacotePremium": 0.0299,
  "taxaDeslocamento": 0
}
```

#### `GET /api/servicos` → lista todos.

### Contratações

#### `POST /api/contratacoes`
```json
// request
{ "clienteId": 10, "servicoId": 1, "valorSolicitado": 5000, "prazoMeses": 12 }

// 202 Accepted
{
  "id": 100,
  "clienteId": 10,
  "servicoId": 1,
  "tipoServico": "PLANO_FUNERARIO",
  "valorSolicitado": 5000.00,
  "prazoMeses": 12,
  "status": "Pendente",
  "solicitadaEm": "2026-05-21T20:00:00Z",
  "processadaEm": null
}
```
- `404 Not Found` se cliente ou serviço não existir
- `400 Bad Request` se serviço inativo

A solicitação é gravada com `Status = Pendente` e publicada na fila in-memory (`InMemoryContratacaoQueue`). O `ContratacaoBackgroundConsumer` (hosted service rodando dentro da API) consome a mensagem, calcula o score e atualiza o registro para `Aprovada`/`Reprovada`/`Falhou`.

#### `GET /api/contratacoes/{id}`
```json
// 200 OK
{
  "id": 100,
  "status": "Aprovada",
  "statusDescricao": "Aprovada",
  "scoreCalculado": 720,
  "taxaAplicada": 0.022,
  "mensagem": "Plano funerário aprovado — mensalidade 2,20% sobre a cobertura",
  "processadaEm": "2026-05-21T20:00:01Z"
}
```

---

## 7. Como executar os testes

```bash
dotnet test Funeraria.sln
```

Saída esperada (rodando localmente):

```
Test run for Funeraria.Tests.dll (.NETCoreApp,Version=v8.0)
A total of 1 test files matched the specified pattern.

Passed!  - Failed: 0, Passed: 26, Skipped: 0, Total: 26, Duration: ~1s
```

> Print do resultado: salve a saída de `dotnet test` em `docs/print-testes.png`.

### Cobertura dos fluxos críticos

| Fluxo crítico                                              | Arquivo                             |
| ---------------------------------------------------------- | ----------------------------------- |
| Cadastro PF/PJ + CPF/CNPJ duplicado                        | `ClientesPfTests`, `ClientesPjTests`|
| Cliente em filial inexistente                              | `ClientesPfTests`                   |
| CPF/CNPJ inválido (400)                                    | `ClientesPfTests`, `ClientesPjTests`|
| Contratação válida → publica na fila + 202                 | `ContratacoesTests`                 |
| Contratação para cliente/serviço inexistente → 404         | `ContratacoesTests`                 |
| Consulta de status após processamento                      | `ContratacoesTests`                 |
| Cálculo de score, regra de plano funerário e taxa premium  | `AnaliseContratacaoServiceTests`    |
| Validação de CPF/CNPJ                                      | `DocumentoValidatorTests`           |

Os testes usam `Microsoft.AspNetCore.Mvc.Testing` + `EF Core InMemory` e um `FakeContratacaoPublisher` para validar a publicação na fila sem depender de RabbitMQ no CI.

---

## 8. Evidência da fila / consumer

Como a entrega usa **fila em memória + `BackgroundService`** no mesmo processo da API, a evidência do processamento assíncrono é o **log da API** mostrando `Contratação {Id} enfileirada para processamento assíncrono` seguido, segundos depois, de `Contratação {Id} processada — status Aprovada score …`. Recomenda-se também tirar print do `GET /api/contratacoes/{id}` antes (status `Pendente`/`EmAnalise`) e depois (`Aprovada`).

> Salvar screenshot em `docs/print-consumer-log.png` (terminal da API com os logs do consumer).

---

## 9. Print do Swagger com contratação aprovada

Roteiro para gerar a evidência:

1. `POST /api/filiais` → cria filial.
2. `POST /api/clientes/pf` → cria cliente com renda mensal alta (ex.: 20.000).
3. `POST /api/servicos/plano-funerario` → cria serviço com `coberturaMaxima: 50000`, `carenciaMaximaMeses: 36`.
4. `POST /api/contratacoes` com valor 5.000 e prazo 12.
5. Aguardar 1–2 segundos (worker processa).
6. `GET /api/contratacoes/{id}` → status `Aprovada`, score ≥ 700, mensalidade < 2,5%.

---

## Anexos

- `docs/diagrama-classes.drawio` — diagrama UML editável.
- `docs/diagrama-classes.puml` — fonte PlantUML.
- `ContratacaoBackgroundConsumer` (em `Funeraria.Infrastructure/Messaging`) — consumer da fila rodando como `BackgroundService` dentro da API.
