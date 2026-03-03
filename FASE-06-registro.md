# FASE 06 — Dashboard administrativo + Gestão (placeholders)

**Data:** ____/____/____  
**Camadas:** `SaphiraTerror.Web` (Área Admin + MVC) · integra `Infrastructure` (AppDbContext/Identity)  
**Objetivo:** entregar o **Dashboard** (cards + gráficos) para **Admin** e a seção **Gestão** (entradas para CRUDs) para **Gerente/Admin**. Os CRUDs completos ficam para a **Fase 07**.

---

## 1) Entregas desta fase

- **Dashboard (AdminOnly)** em `/Admin/Dashboard`:
  - **Cards**: Total de filmes, total de gêneros, total de usuários, usuários por perfil.
  - **Gráficos**: filmes por **gênero**, por **classificação** e por **ano** (Chart.js).
  - **Lista**: 5 últimos filmes cadastrados.
- **Gestão (ManagerOrAdmin)** em `/Admin/Manage`:
  - Página de **entrada** para os módulos de **Filmes**, **Gêneros**, **Classificações** e **Usuários**.
  - **Placeholders** de views para cada módulo (CRUDs implementados na Fase 07).
- Serviços **enxutos** usando **AppDbContext** diretamente (consultas de leitura e agrupamentos).
- **Sem** CSS inline novo; scripts de gráficos centralizados em `wwwroot/js/dashboard.js`.

---

## 2) Arquivos criados/atualizados (caminhos completos)

> Em conformidade com nosso padrão: **CRIAR/ATUALIZAR** + arquivo **inteiro**.

### 2.1 Serviço do Dashboard

- **CRIAR** `SaphiraTerror.Web/Areas/Admin/Services/DashboardService.cs`  
  - Métodos:
    - `TotalFilmesAsync`, `TotalGenerosAsync`, `TotalUsuariosAsync`
    - `TotalUsuariosPorRoleAsync()`
    - `FilmesPorGeneroAsync()`, `FilmesPorClassificacaoAsync()`, `FilmesPorAnoAsync()`
    - `UltimosFilmesAsync(take=5)`

### 2.2 Controller do Dashboard

- **ATUALIZAR** `SaphiraTerror.Web/Areas/Admin/Controllers/DashboardController.cs`  
  - `[Authorize(Policy="AdminOnly")]`  
  - `Index()` injeta dados via `ViewBag` e renderiza `~/Areas/Admin/Views/Dashboard/Index.cshtml`  
  - Endpoint JSON `GET Admin/Dashboard/Data` para carregar os gráficos (labels + data)

### 2.3 View do Dashboard

- **CRIAR** `SaphiraTerror.Web/Areas/Admin/Views/Dashboard/Index.cshtml`  
  - Cards de totais + quadro “Usuários por perfil”  
  - 3 gráficos (`chartGenero`, `chartClass`, `chartAno`)  
  - Lista “5 últimos filmes”  
  - Inclusão de `Chart.js` (CDN) e `~/js/dashboard.js`

### 2.4 Script dos gráficos

- **CRIAR** `SaphiraTerror.Web/wwwroot/js/dashboard.js`  
  - `fetch('/Admin/Dashboard/Data')` e renderização com `Chart.js` (bar/line)

### 2.5 Registro no DI

- **ATUALIZAR** `SaphiraTerror.Web/Program.cs`  
  - **Somente** adicionar:
    ```csharp
    using SaphiraTerror.Web.Areas.Admin.Services;
    // ...
    builder.Services.AddScoped<DashboardService>();
    ```
  - Demais serviços/policies permaneceram iguais aos da Fase 05.

### 2.6 Ajustes de imports da área

- **ATUALIZAR** `SaphiraTerror.Web/Areas/Admin/Views/_ViewImports.cshtml`  
  - Garantir:
    ```cshtml
    @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
    @using SaphiraTerror.Web
    @using SaphiraTerror.Web.Areas.Admin.Services
    ```

### 2.7 Gestão (placeholders)

- **ATUALIZAR** `SaphiraTerror.Web/Areas/Admin/Controllers/ManageController.cs`  
  - `[Authorize(Policy="ManagerOrAdmin")]`  
  - Ações: `Index`, `Filmes`, `Generos`, `Classificacoes`, `Usuarios` (todas GET)
- **CRIAR** `SaphiraTerror.Web/Areas/Admin/Views/Manage/Index.cshtml`  
  - Cartões de navegação para os 4 módulos
- **CRIAR** `SaphiraTerror.Web/Areas/Admin/Views/Manage/Filmes.cshtml`  
- **CRIAR** `SaphiraTerror.Web/Areas/Admin/Views/Manage/Generos.cshtml`  
- **CRIAR** `SaphiraTerror.Web/Areas/Admin/Views/Manage/Classificacoes.cshtml`  
- **CRIAR** `SaphiraTerror.Web/Areas/Admin/Views/Manage/Usuarios.cshtml`  
  - Todas como **placeholders** (tabelas vazias) aguardando a Fase 07

> **Observação**: o `_Layout.cshtml` foi ajustado previamente na Fase 05 para usar `asp-area=""` no Logout/Login; nenhuma alteração adicional nesta fase.

---

## 3) Rotas e policies (recapitulação)

- **Rotas de áreas** (já existentes):
  ```csharp
  app.MapControllerRoute("areas",
      "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
