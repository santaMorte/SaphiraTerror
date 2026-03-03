
---

## `docs/fases/FASE-02-registro.md`
```markdown
# Fase 2 — Repositórios & Serviços (filtros + paginação) (data: ____/____/____)

**Objetivo**  
Padronizar contratos (DTOs/filtros), criar repositórios EF e serviços de consulta com filtros/ordenação/paginação e registrar no DI.

## Escopo (entra)
- **DTOs (records)**: `PagedRequest`, `PagedResult<T>`, `FilmeDto`
- **Filtro (record)**: `FilmeFilter : PagedRequest`  
  Campos: `GeneroId?`, `ClassificacaoId?`, `Ano?`, `Q`, `SortBy`, `Desc`, `Page`, `PageSize`
- **Repositórios (Application/Abstractions)**:
  - `IFilmeRepository`, `IGeneroRepository`, `IClassificacaoRepository`
- **Infra (implementação EF)**:
  - `EfFilmeRepository` (com `AsNoTracking`, `Include`, `EF.Functions.Like`, ordenação e paginação)
  - `EfGeneroRepository`, `EfClassificacaoRepository`
- **Serviços (Application ? Infra)**:
  - `IFilmeQueryService` + `FilmeQueryService` (mapeia para `FilmeDto`)
  - `ICatalogLookupService` + `CatalogLookupService` (tuplas `(Id, Nome)`)
- **DI**: registro dos repos/serviços em `AddInfrastructure(...)`

## Entregáveis (detalhe técnico)
- **Ordenação**: `SortBy` = `"Titulo" | "Ano" | "CreatedAt"`; padrão `CreatedAt DESC`
- **Busca textual**: `Q` busca em `Titulo` e `Sinopse` usando `LIKE` (`%term%`)
- **Paginação**: `Page>=1`; `PageSize` saneado para `[1..48]`, padrão 12
- **Projeção**: serviço retorna `PagedResult<FilmeDto>` (somente dados necessários ao front)
- **Performance**: `AsNoTracking` em leitura; `Include` apenas de `Genero` e `Classificacao`

## Recursos aplicados (por que conta)
- **records** para DTOs e filtros ? imutáveis leves, igualdade por valor
- **contratos no Application** ? facilita testes/mocks e troca de infraestrutura
- **clamp de PageSize** ? proteção simples contra requisições pesadas

## Como rodar (validação)
```bat
dotnet build
dotnet run --project SaphiraTerror.Api
:: (sem endpoints ainda — valida build limpo e DI resolvendo tudo)
