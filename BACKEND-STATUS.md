# NEXORA Backend — status desta entrega

## Implementado nesta versão
- Ingestão de eventos com validação de campos, payload JSON, data não futura e idempotência por sistema + EventId.
- Persistência do payload JSON serializado na coluna PostgreSQL `text`, compatível com o schema atual do banco `nexora_db`.
- Análise independente de módulo externo: divergência de valores financeiros e quantidades entre sistemas.
- Lista opcional de sistemas esperados via `Monitoring:ExpectedSystems`.
- Criação/atualização de alertas com histórico.
- Consultas de eventos, alertas, histórico, prioridades, recorrências, dashboard e saúde/desempenho dos sistemas.
- Paginação e filtros de alertas.
- Segredos removidos do `appsettings.json`; conexão via User Secrets ou variável de ambiente.
- Exemplos de chamadas em `Nexora.Api.http`.

## Ainda exige validação no computador de desenvolvimento
Este ambiente não possui .NET SDK nem PostgreSQL, portanto não foi possível executar `dotnet restore`, `dotnet build`, migrations, nem testes de integração reais. Não seria correto declarar o backend 100% validado sem essas etapas.

O banco existente foi inspecionado pelo usuário: possui `Events`, `Alerts`, `AlertHistories` e `__EFMigrationsHistory`. A coluna `Events.Data` é `text`; o mapeamento EF Core foi alinhado a esse tipo. `EnsureCreated` permanece apenas como bootstrap para banco vazio e não altera o schema existente. Não foi feita conversão de tipo nem exclusão de dados.

## Fora do escopo desta entrega
- Autenticação/autorização e gestão de chaves de API para integrações externas.
- Fila assíncrona, retentativas e processamento distribuído.
- Configuração de regras por cliente/empresa e por tipo de evento.
- Integração com interface web/mobile.
- Monitoramento de ausência de evento por janela de tempo: a lista de sistemas esperados detecta ausência no conjunto já recebido para um registro/tipo; alertas temporais de timeout exigem uma política de prazo por integração.

## Revisão estática adicional
- Validação explícita dos comprimentos dos identificadores antes da persistência.
- Conflitos de idempotência agora retornam HTTP 409 somente para violação de índice único; falhas de banco não são mascaradas como duplicidade.
- Ordenação paginada determinística e proteção contra overflow no cálculo do offset.
- Percentual de alertas por sistema calculado por combinação distinta de registro/tipo afetada, evitando percentuais acima de 100% por reincidência.
- A revisão foi estática: sem SDK .NET neste ambiente, ainda não houve build nem teste de integração.
