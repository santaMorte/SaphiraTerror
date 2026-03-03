# Fase 0 — Scaffold & Fundamentos (data: ____/____/____)

**Objetivo**  
Criar a solução .NET 9 com 5 projetos, subir API e Web mínimas e preparar terreno para evoluir sem retrabalho.

## Escopo (entra)
- Solução `SaphiraTerror.sln`
- Projetos: `Domain`, `Application`, `Infrastructure`, `Api`, `Web`
- Navbar fixa e páginas base no Web (Home, About, Catalog, Genre, Logout)
- Endpoint de saúde na API (`GET /` ? `{ name, status }`)
- Tema light/dark simples via `data-bs-theme` (Bootstrap 5.3)

## Escopo (não entra)
- Banco/EF/Migrations/Seeds
- Auth e Roles
- Endpoints REST reais

## Entregáveis (arquivos-chave)
- **Api**: `Program.cs` com `MapGet("/")` e CORS aberto provisório
- **Web**: `Program.cs`, `Controllers/HomeController.cs`, `Views/Shared/_Layout.cshtml` (navbar+toggle), `Views/Home/*.cshtml`
- **Raiz**: `global.json` com SDK .NET 9 (ou `rollForward`)

## Recursos aplicados (por que conta)
- **Pin de SDK** (`global.json`) ? evita “na minha máquina funciona”
- **Bootstrap 5.3 via CDN** ? velocidade e padrão responsivo
- **Toggle light/dark** com `localStorage` ? UX moderna e simples
- **CORS liberado só por enquanto** ? desenvolvimento sem fricção

## Como rodar (Windows)
```bat
dotnet build
dotnet run --project SaphiraTerror.Api
:: abrir http://localhost:5203/  ? { "name": "SaphiraTerror.Api", "status": "ok" }

dotnet run --project SaphiraTerror.Web
:: abrir a Home com navbar e tema
