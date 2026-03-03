# Fase 07 — Gestão de Usuários (CRUD + Soft Delete + Roles)

## ?? Objetivo
Implementar o módulo administrativo de **Usuários** com:
- Listagem com filtros (texto, role, status).
- **Create / Edit** com atribuição de **uma role** por usuário.
- **Soft delete (Desativar)** e **Reativar**.
- **Regra de segurança:** somente **Admin** pode desativar/reativar (Gerente não pode).

---

## ?? Escopo implementado

### Web (Área Admin)
- **Controllers**
  - `SaphiraTerror.Web/Areas/Admin/Controllers/UsuariosController.cs`
    - Index lista **ativos e inativos** usando `_db.Users.IgnoreQueryFilters()`.
    - Filtros: `Q` (email/username), `Role`, `Ativo`.
    - Create/Edit com validação de role e normalização (`NormalizedEmail/UserName`).
    - **[AdminOnly]** em `Desativar` e `Reativar` + `Forbid()` (defesa extra).
- **Views**
  - `.../Views/Usuarios/Index.cshtml` — tabela + filtros + paginação; botões “Desativar/Reativar”.
  - `.../Views/Usuarios/Create.cshtml` — criação com escolha de role.
  - `.../Views/Usuarios/Edit.cshtml` — edição + troca de role.
- **Models (VMs)**
  - `SaphiraTerror.Web/Areas/Admin/Models/UsuarioVms.cs`

### Policies / Auth (Program)
- `Program.cs`
  - Identity com **roles Guid**: `.AddRoles<IdentityRole<Guid>>()`.
  - Policies:
    - `AdminOnly` ? somente Admin/ADMIN/Administrador.
    - `ManagerOrAdmin` ? Gerente ou Admin.
  - Cookies: rotas de `Login`, `Logout`, `Denied`.

### Seeder (idempotente)
- `SaphiraTerror.Infrastructure/Persistence/Seed/DatabaseSeeder.cs`
  - **Roles** garantidas: Admin, Gerente, Outros (com `NormalizedName`).
  - **Usuários padrão**: admin@…, gerente@…, usuario@… (sem duplicar).
  - Busca por role via `NormalizedName` (compatível com EF).
  - `using Microsoft.Extensions.DependencyInjection;` para `CreateScope()`.

---

## ?? Regras de Acesso
- **Listar/Editar/Criar**: `ManagerOrAdmin`.
- **Desativar/Reativar (soft delete)**: **somente `AdminOnly`**.
- Botões de desativar/reativar **podem** ficar ocultos no front para não-admins (UX), mas a segurança é garantida no controller (server-side).

---

## ?? Testes manuais
1. **Admin**:
   - Criar usuário em cada role (Admin/Gerente/Outros).
   - Editar usuário e trocar role.
   - Desativar usuário ? aparece como **Inativo** na listagem (filtro “Ativo: Todos”).
   - Reativar usuário.
   - Não permitir desativar a si próprio.
2. **Gerente**:
   - Acessar listagem/edição/criação.
   - Tentar **desativar/reativar** ? **Acesso negado**.
3. **API**:
   - Subir API + Web; seeder não duplica (idempotente).
4. **Filtros**:
   - Buscar por email/username.
   - Filtrar por Role.
   - Filtrar por Status (Ativo / Não / Todos).

---

## ?? Observações técnicas
- Como existe *query filter global* para `ApplicationUser`, usamos `IgnoreQueryFilters()` nas telas de gestão — assim **inativos aparecem**.
- Para roles na listagem, lemos `AspNetUserRoles`/`AspNetRoles` direto (evita depender do `UserManager` e ignora filtros).
- `IdentityRole` é **genérico com `Guid`**; alinhar `RoleManager<IdentityRole<Guid>>` no DI e nos controllers.
- Evitar `string.Equals(..., StringComparison)` em LINQ para EF; preferir `NormalizedName`/`NormalizedEmail`.

---

## ? Done / Entregáveis
- Gestão de **Usuários** com soft delete/reativar e controle de roles.
- Segurança e UX revisadas.
- Seeder idempotente (pode rodar na Web e na API).

