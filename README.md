# NEXORA — Backend API

Backend em ASP.NET Core 10 + Entity Framework Core + PostgreSQL.

## O que está implementado

- Recepção de eventos externos com validação e idempotência por `SystemName + EventId`.
- Armazenamento do payload JSON original na coluna PostgreSQL `text` (compatível com o banco existente).
- Detecção de divergências financeiras e de quantidade entre sistemas usando `amount`/`valorTotal`/`totalAmount` e `quantity`/`quantidade`/`totalQuantity` no JSON `data`.
- Criação/atualização de alertas e trilha de histórico.
- Consulta paginada de eventos e alertas.
- Priorização, resolução e histórico de alertas.
- Dashboard, reincidências, desempenho e resumo da saúde dos sistemas.
- Endpoint de status com verificação de conexão ao PostgreSQL.
- Configuração de segredo fora do repositório.

## Requisitos

- .NET SDK 10.0.
- PostgreSQL instalado e em execução.
- Rider ou Visual Studio Code (opcional).

## Configurar o PostgreSQL

O banco `nexora_db` já existe na instalação que estamos preparando. **Não o apague nem o recrie.** A estrutura existente inspecionada contém `Events`, `Alerts`, `AlertHistories` e `__EFMigrationsHistory`. A coluna `Events.Data` é `text`, por isso o modelo EF também usa `text` e grava o JSON serializado como texto.

Configure a conexão com User Secrets (recomendado), dentro da pasta `Nexora.Api`:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=1234;Database=nexora_db;Username=postgres;Password=SUA_SENHA"
```

Ajuste `Port` e `Password` para a sua instalação. Não coloque senhas em `appsettings.json`, no GitHub ou em prints.

**Segurança:** a versão anterior do repositório continha uma senha de banco em texto puro. Considere essa senha comprometida e troque-a no PostgreSQL antes de executar esta versão.

## Configurar sistemas monitorados (opcional)

Edite `Monitoring:ExpectedSystems` em `appsettings.json` com os nomes exatos dos sistemas esperados para cada registro/tipo analisado. A configuração atual é uma lista global, aplicada a todos os tipos de evento; se diferentes eventos tiverem conjuntos de sistemas diferentes, essa configuração deve evoluir para regras por tipo. Exemplo: `["SistemaVendas", "SistemaEstoque", "SistemaFinanceiro"]`. Deixe vazio até definir a matriz real de integrações; caso contrário, a NEXORA pode sinalizar ausências incorretamente.

## Executar

Na pasta `Nexora.Api`:

```bash
dotnet restore
dotnet build
dotnet run
```

O perfil HTTP padrão usa `http://localhost:5241`. A API mantém `EnsureCreated` apenas como bootstrap para um banco realmente vazio. No banco atual, que já possui tabelas, `EnsureCreated` não aplica migrações nem atualiza tabelas ou índices existentes. O mapeamento de `Events.Data` foi ajustado para `text`, que é o tipo encontrado no banco. Antes de usar em produção, compare todas as colunas, constraints e índices do banco com o modelo; o histórico de migrações, sozinho, não garante que o schema atual corresponda ao código.

> Importante: esta versão foi adaptada para ler e gravar no schema atual sem conversão destrutiva. Evoluções futuras do schema devem ser feitas com migrations EF Core versionadas e revisadas. Não converta `Events.Data` para `jsonb` automaticamente, pois isso exigiria validar todos os registros existentes primeiro.

## Testar

Abra `Nexora.Api.http` no Rider e execute as requisições em sequência. Envie primeiro o evento de vendas e depois o evento financeiro com o mesmo `recordId` e `eventType`, mas valores diferentes. A segunda resposta deve trazer uma inconsistência detectada e criar um alerta.

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/status` | Estado da API e banco |
| POST | `/api/events` | Receber evento e analisar o registro |
| GET | `/api/events?page=1&pageSize=50` | Listar eventos |
| GET | `/api/events/{id}` | Consultar evento |
| GET | `/api/events/anomalies` | Consultar anomalias (somente leitura) |
| GET | `/api/alerts` | Listar alertas; aceita `resolved=true/false` |
| GET | `/api/alerts/prioritized` | Alertas pendentes ordenados por prioridade |
| PATCH | `/api/alerts/{id}/resolve` | Resolver alerta |
| GET | `/api/alerts/{id}/history` | Histórico de um alerta |
| GET | `/api/dashboard/summary` | Indicadores gerais |
| GET | `/api/alerts/recurrences` | Reincidências |
| GET | `/api/systems/performance` | Desempenho por sistema |
| GET | `/api/systems/health` | Resumo da saúde dos sistemas |

## Limites atuais / próxima integração

A análise implementada é independente de qualquer integrante: compara o evento mais recente por sistema para o mesmo `recordId` e `eventType`, verificando divergências financeiras e de quantidade. Também pode sinalizar sistemas esperados ausentes quando a lista `Monitoring:ExpectedSystems` estiver configurada. A API rejeita IDs de evento duplicados e datas inválidas/futuras. A detecção de sistema ausente só é confiável quando a organização configurar quais sistemas devem enviar cada tipo de evento; na configuração padrão a lista está vazia para evitar falsos positivos. Regras de negócio específicas de pedidos/estoque podem ser ampliadas depois sem depender do módulo externo.

A compilação e os testes de integração devem ser executados no computador de desenvolvimento. O ambiente em que esta revisão foi preparada não possui .NET SDK nem acesso ao PostgreSQL local do usuário.
