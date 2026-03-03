# FASE 05 — Web com Identity (cookies) + Áreas e Policies

**Data:** ____/____/____  
**Camadas:** `SaphiraTerror.Web` (MVC) + `Infrastructure` (DbContext/Identity)  
**Objetivo:** o **Web** passa a autenticar **diretamente** com ASP.NET Identity (cookies) usando **o mesmo `AppDbContext`** da solução. O consumo da **API** continua **apenas** para o catálogo.  
**Policies:**  
- `AdminOnly` ? exige **Admin**  
- `ManagerOrAdmin` ? exige **Gerente** **ou** **Admin**

---

## 1) Entregas da fase

- Web usa **Identity (cookies)** com `ApplicationUser` e **EF Stores** apontando para o **mesmo SQL Server** do projeto.
- **Login/Logout** via `AuthController` (`/Auth/Login`, `/Auth/Logout`).
- **Área Admin** (rotas de área):
  - `/Admin/Dashboard` ? `AdminOnly`
  - `/Admin/Manage`     ? `ManagerOrAdmin`
- **Policies** configuradas conforme **nomes de roles do banco** (Admin/Gerente).
- Menu exibe **Dashboard** e **Gestão** **somente** quando o usuário tem as roles.

> Observação: o **catálogo** continua vindo da **API** via `IApiClient` (sem alterações aqui).

---

## 2) Arquivos criados/atualizados

> Ações seguem o padrão **CRIAR/ATUALIZAR** c/ caminho completo.

### 2.1 Configuração do Web

- **ATUALIZAR** `SaphiraTerror.Web/Program.cs`  
  **Antes** do `Build()`:
  - MVC, MemoryCache, `HttpClient<IApiClient>` (ApiBaseUrl em `appsettings`).
  - `AddDbContext<AppDbContext>(UseSqlServer)`.
  - `AddIdentityCore<ApplicationUser>()` + `AddRoles<IdentityRole<Guid>>()` + `AddEntityFrameworkStores<AppDbContext>()` + `AddSignInManager()`.
  - Cookies (`LoginPath`, `LogoutPath`, `AccessDeniedPath`).
  - **Policies** (ajustadas aos nomes do seu banco):
    ```csharp
    builder.Services.AddAuthorization(opt =>
    {
        opt.AddPolicy("AdminOnly",      p => p.RequireRole("Admin"));
        opt.AddPolicy("ManagerOrAdmin", p => p.RequireRole("Admin", "Gerente"));
    });
    ```
  Pipeline:
  ```csharp
  app.UseStaticFiles();
  app.UseRouting();
  app.UseAuthentication();
  app.UseAuthorization();

  // áreas
  app.MapControllerRoute("areas",
      "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

  // default
  app.MapControllerRoute("default",
      "{controller=Home}/{action=Index}/{id?}");
