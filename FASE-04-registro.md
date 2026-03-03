# Fase 4 — Web (Catálogo + Filtros) — one-page (data: ____/____/____)

**Objetivo**  
Entregar a camada Web consumindo a API: catálogo com cards, filtros (gênero, classificação, ano, texto), ordenação e paginação. Unificar navegação em **uma página (one-page)** com seções (Home, About, Genres, Catalog) e manter compatibilidade com rotas antigas.

---

## Escopo (entra)
- `SaphiraTerror.Web`:
  - **HttpClient** tipado para chamar a API (`ApiBaseUrl` por config).
  - **DTOs/ViewModels** para receber `PagedResult<FilmeDto>`.
  - **Serviço `IApiClient/ApiClient`** com cache leve (`IMemoryCache`) para gêneros/classificações.
  - **One-page**: `Home/Index` com seções `#home`, `#about`, `#genres` (cards), `#catalog` (catálogo completo).
  - **Partial `_CatalogSection`** contendo filtros, cards e paginação (reuso fácil).
  - **Navbar** com âncoras + **footer** (site map).
  - **Compatibilidade**: `/Home/Catalog` e `/Home/Genre` continuam válidos, renderizando a Index e rolando para a seção certa.
- **Sem mudanças na API/Infrastructure/Application** (reuso total).

## Escopo (não entra)
- Login/logout e proteção por perfil (Fase 5).
- Dashboard/CRUD (Fase 6).
- “Ver mais”/modal de detalhes (poderá vir como refinamento).

---

## Entregáveis (arquivos criados/alterados)
**Config**
- `SaphiraTerror.Web/appsettings.Development.json`  
  `{"ApiBaseUrl":"http://localhost:5203/"}`
- `SaphiraTerror.Web/Program.cs`  
  MVC + `AddMemoryCache` + `AddHttpClient<IApiClient, ApiClient>` + JSON options.

**Models (Web)**
- `Models/Dtos.cs` ? `record FilmeDto`, `record PagedResult<T>`.
- `Models/CatalogModels.cs` ? `CatalogFilterVm`, `CatalogPageVm`.

**Serviços (Web)**
- `Services/IApiClient.cs`  
- `Services/ApiClient.cs` (métodos: `GetGenerosAsync`, `GetClassificacoesAsync`, `SearchFilmesAsync`, `GetFilmeAsync`).

**Controllers (Web)**
- `Controllers/HomeController.cs`  
  - `Index([FromQuery] CatalogFilterVm, section)` ? **one-page**.  
  - Rotas compatíveis: `Catalog(...)` ? `Index(..., "catalog")`; `Genre()` ? `Index(..., "genres")`.

**Views**
- `Views/Shared/_Layout.cshtml` ? Navbar com âncoras `/#home`, `/#about`, `/#genres`, `/#catalog` e link “Área restrita” (`/Auth/Login`).
- `Views/Home/Index.cshtml` ? seções **Home/About/Genres/Catalog**.  
  `Genres`: cards dinâmicos; clique preenche `GeneroId` no form e envia para o catálogo.  
  `Catalog`: inclui partial `_CatalogSection`.
- `Views/Shared/_CatalogSection.cshtml` ? filtros (gênero/classificação/ano/busca/ordenar/itens), **cards com sinopse**, paginação renderizada (mostra “Página X de Y” e **total**).

**Estilo/UX**
- Cards com `hover`, navbar sticky, tema light/dark preservado, footer com site map.

---

## Recursos aplicados (por que conta)
- **One-page**: fluxo mais direto (conteúdo curto), sem páginas “vazias”.
- **Compatibilidade de rotas**: `/Home/Catalog` e `/Home/Genre` ainda funcionam (útil para materiais antigos).
- **Cache leve** em `IApiClient` (10 min) para gêneros/classificações ? menos chamadas.
- **Saneamento de paginação** no controller (Page >= 1; PageSize ? 48).
- **Sinopse** exibida nos cards (visível ao usuário; futuro: modal “ver mais”).
- **Razor sem C# em atributos** (`<option selected>` via condicionais) ? evita erros `RZ1031`.

---

## Como rodar
```bat
:: 1) API (porta fixa)
dotnet run --project SaphiraTerror.Api --urls http://localhost:5203

:: 2) Web (porta fixa)
dotnet run --project SaphiraTerror.Web --urls http://localhost:5000

:: 3) Acessar
http://localhost:5000/          (one-page)
http://localhost:5203/swagger    (API)
