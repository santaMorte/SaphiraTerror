
---

## `docs/fases/FASE-01-registro.md`
```markdown
# Fase 1 — Domínio + EF Core (SQL Server) + Migrations + Seed (data: ____/____/____)

**Objetivo**  
Modelar entidades, configurar EF/Identity, criar banco e popular dados iniciais (idempotente).

## Escopo (entra)
- **Domain/Entities**: `Filme`, `Genero`, `Classificacao`, `TipoUsuario`
- **Infrastructure**:
  - `ApplicationUser : IdentityUser<Guid>` (campos: IsActive, TipoUsuarioId, auditoria)
  - `AppDbContext` (Fluent API, índices e relacionamentos)
  - `DatabaseSeeder` (migrate + seeds idempotentes)
  - `DesignTimeDbContextFactory` (para dotnet-ef)
- **Api**:
  - `Program.cs` chama `AddInfrastructure(...)` e `DatabaseSeeder.EnsureSeededAsync(...)`
  - `appsettings.Development.json` com `DefaultConnection` (SQL Server/LocalDB)

## Entregáveis (detalhe técnico)
- **Relacionamentos:** `Filme (N:1) Genero`, `Filme (N:1) Classificacao` com `DeleteBehavior.Restrict`
- **Índices:** `Filme(Titulo)`, `Filme(Ano)`, `Filme(GeneroId,ClassificacaoId)`
- **Filtro global (Identity):** `ApplicationUser` somente `IsActive = true`
- **Seed idempotente:**
  - `TiposUsuario` (Administrador/Gerente/Outros) **sem setar Id manual**
  - **Roles**: Admin, Gerente, Outros (criadas antes do usuário)
  - **Usuários padrão**:
    - `admin@saphira.local / Admin@123` (Admin)
    - `gerente@saphira.local / Gerente@123` (Gerente)
    - `usuario@saphira.local / Usuario@123` (Outros)
  - **Catálogos**: Gêneros, Classificações
  - **Filmes**: lote inicial + extras por gênero (inserção condicional por Título)

## Recursos aplicados (por que conta)
- **Fluent API** no `DbContext` ? domínio limpo (sem DataAnnotations)
- **Seeds idempotentes** ? repetir start não duplica dados
- **Guid em Identity** ? chaves consistentes para usuários
- **Restrict em FK** ? evita cascatas acidentais

## Como rodar
```bat
dotnet tool restore

:: (Se necessário gerar/atualizar)
dotnet ef migrations add InitialCreate ^
  -p SaphiraTerror.Infrastructure -s SaphiraTerror.Api ^
  -o Persistence/Migrations

dotnet ef database update ^
  -p SaphiraTerror.Infrastructure -s SaphiraTerror.Api

dotnet run --project SaphiraTerror.Api
