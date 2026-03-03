# SaphiraTerror — Visão Geral da Solução
*(data: __/__/____)*

**Objetivo**  
Dar um **mapa rápido** do projeto para que qualquer aluno saiba **onde olhar** e **onde mexer** sem se perder.

---

## ?? Mapa da Solução (projetos & papéis)

| Projeto | Papel | O que contém (exemplos) | Quem pode chamar |
|---|---|---|---|
| **SaphiraTerror.Domain** | **Núcleo de negócio** | Entidades, enums, regras puras | Application, Infrastructure, Api |
| **SaphiraTerror.Application** | **Casos de uso** | DTOs, filtros, contratos de repositório, serviços | Api (controladores), Web (indiretamente via Api) |
| **SaphiraTerror.Infrastructure** | **Técnico/Persistência** | `AppDbContext (EF)`, mappings, repositórios concretos, Identity, Seed | Api (DI), Web (Identity/EF p/ Admin) |
| **SaphiraTerror.Api** | **Back-end HTTP** | Endpoints REST (Filmes/Gêneros/Classificações), validação, Swagger | Consumido pelo Web para catálogo |
| **SaphiraTerror.Web** | **Front MVC** | One-page (Home/About/Catalog/Genre), Área Admin (Gestão), Views/Controllers/Assets | Chama **Api** (catálogo) e usa **Identity/EF** (Admin) |

> **Regra de ouro:** **Domain** não referencia ninguém.  
> **Application** referencia apenas **Domain**.  
> **Infrastructure** implementa **Application** (repositórios/EF).  
> **Api** expõe HTTP usando Application/Infrastructure.  
> **Web** consome **Api** no catálogo e usa **Identity/EF** direto na área **Admin**.

---

## ?? Comunicação (fluxos principais)

### 1) Catálogo (público)

Browser ? SaphiraTerror.Web (HomeController)
? (HttpClient via IApiClient)
SaphiraTerror.Api ? Application ? Infrastructure (EF) ? SQL Server
? JSON
Browser (renderiza cards, filtros, paginação)


### 2) Gestão (Área Admin)


Browser ? SaphiraTerror.Web (Areas/Admin)
? (Identity/EF direto no Web)
Infrastructure (DbContext/Identity) ? SQL Server


---

## ?? Dependências (quem acessa quem)

Domain
?
?
Application ??? (Interfaces de repositório)
? ?
? ?
Api ???????????????????
?
?
Infrastructure (implementa repositórios/EF/Identity/Seed)

Web ?? Api (HttpClient/IApiClient para catálogo)
Web ?? Infrastructure (Identity/EF para Admin)


- **Proibido**: Domain depender de qualquer outro projeto.  
- **Evitar**: Api falar com EF direto sem passar pelos repositórios/serviços do Application.

---

## ??? Onde mexer (guia rápido)

### Layout/estilo da Home
- `SaphiraTerror.Web/Views/Home/Index.cshtml`  
- `SaphiraTerror.Web/Views/Shared/_Layout.cshtml`  
- `SaphiraTerror.Web/wwwroot/css/site.css`

### Filtros/paginação do catálogo
- Web Controller: `SaphiraTerror.Web/Controllers/HomeController.cs`  
- Cliente HTTP: `SaphiraTerror.Web/Web.Services/ApiClient.cs`  
- Api Controller: `SaphiraTerror.Api/Controllers/FilmesController.cs`

### CRUDs (Admin)
- Views: `SaphiraTerror.Web/Areas/Admin/Views/**`  
- Controllers: `SaphiraTerror.Web/Areas/Admin/Controllers/**`  
- EF/DbContext: `SaphiraTerror.Infrastructure/Persistence/AppDbContext.cs`  
- Seeds: `SaphiraTerror.Infrastructure/Persistence/Seed/DatabaseSeeder.cs`

### Login/Permissões (Identity)
- Políticas/Paths: `SaphiraTerror.Web/Program.cs`  
- Usuário: `SaphiraTerror.Infrastructure/Entities/ApplicationUser.cs`  
- Views: `SaphiraTerror.Web/Areas/Admin/Views/Usuarios/**` (e/ou `Views/Auth/**`)

### Endpoint novo
- Api: `SaphiraTerror.Api/Controllers/...`  
- Application: serviço/contrato (se necessário)  
- Infrastructure: repositório/consulta EF (se necessário)  
- Web: chamada via `IApiClient` e renderização.

---

## ?? Como criar **entidade nova** (roteiro)

1. **Domain** ? `Entities/MinhaEntidade.cs`  
2. **Infrastructure**  
   - `Persistence/AppDbContext.cs` (DbSet)  
   - `Persistence/Mappings/MinhaEntidadeMap.cs` (Fluent)  
   - `Persistence/Repositories/MinhaEntidadeRepository.cs` (implementação)  
3. **Application**  
   - `Abstractions/Repositories/IMinhaEntidadeRepository.cs`  
   - `Services/MinhaEntidadeService.cs` (opcional)  
4. **Api** ? `Controllers/MinhaEntidadeController.cs`  
5. **Migrations**  
   ```bash
   dotnet ef migrations add AddMinhaEntidade \
     -p SaphiraTerror.Infrastructure -s SaphiraTerror.Api \
     -o Persistence/Migrations

   dotnet ef database update \
     -p SaphiraTerror.Infrastructure -s SaphiraTerror.Api

?? Políticas de autorização (onde ficam/como usar)

Definição: SaphiraTerror.Web/Program.cs

AdminOnly: aceita “ADMIN”, “Admin”, “Administrador”

ManagerOrAdmin: idem + “GERENTE”, “Gerente”

Uso: Em controllers/actions/views de Admin ? [Authorize(Policy="AdminOnly")].

?? Como rodar (dev)
# Subir API (deixe rodando)
dotnet run --project SaphiraTerror.Api

# Subir Web
dotnet run --project SaphiraTerror.Web


API: http://localhost:5000/ (saúde)

Web: porta informada no console (navbar, catálogo, área Admin)

Logins seed (padrão):

Admin: admin@saphira.local / Admin@123

Gerente: gerente@saphira.local / Gerente@123

Outros: usuario@saphira.local / Usuario@123

? Checklist (sinais de que tudo ok)

 Webb abre com navbar/tema e carrega o catálogo da API

 API lista filmes com filtros/paginação

 Área Admin autentica e aplica policies (AdminOnly / ManagerOrAdmin)

 Soft delete de usuário bloqueado para Gerente (somente Admin)

 Seeds (roles/usuários básicos/gêneros/classificações/filmes) aplicados

?? Problemas comuns & correções rápidas

“Cadastrei um gênero e o card não apareceu na Home.”

O catálogo pode estar em cache no IApiClient. Reinicie o Web ou limpe o cache após salvar um gênero (a action Admin pode invalidar).

“Bloqueio ao excluir gênero/classificação.”

Existem filmes vinculados. Exibimos TempData["warn"] na View explicando o motivo.

“Gerente consegue inativar usuário.”

Verifique policy no controller: ações de inativar devem estar em AdminOnly.

“Falha no login/roles não reconhecidas.”

Confira se o seed criou as roles (ADMIN, GERENTE) e se o usuário correto está adicionado a elas.

?? Glossário

Domain: coração do sistema (regras/entidades).

Application: casos de uso (orquestra Domain).

Infrastructure: EF Core, repositórios, Identity, Seed.

Api: portas HTTP.

Web: MVC (público + Admin).