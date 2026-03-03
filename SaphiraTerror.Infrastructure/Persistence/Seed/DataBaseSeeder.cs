//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using SaphiraTerror.Domain.Entities;
//using SaphiraTerror.Infrastructure.Entities;

//namespace SaphiraTerror.Infrastructure.Persistence.Seed
//{
//    /// <summary>
//    /// Aplica migrations e insere dados iniciais de forma idempotente.
//    /// Ordem: Migrate -> TiposUsuario -> Roles -> Usuários padrão -> Catálogos (Gênero, Classificação, Filmes)
//    /// </summary>
//    public static class DatabaseSeeder
//    {
//        public static async Task EnsureSeededAsync(IServiceProvider services)
//        {
//            using var scope = services.CreateScope();
//            var sp = scope.ServiceProvider;

//            var ctx = sp.GetRequiredService<AppDbContext>();
//            await ctx.Database.MigrateAsync();

//            // =======================
//            // Tipos de usuário (sem setar Id manual para não colidir com IDENTITY)
//            // =======================
//            if (!await ctx.TiposUsuario.AnyAsync())
//            {
//                ctx.TiposUsuario.AddRange(
//                    new TipoUsuario { Descricao = "Administrador" },
//                    new TipoUsuario { Descricao = "Gerente" },
//                    new TipoUsuario { Descricao = "Outros" }
//                );
//                await ctx.SaveChangesAsync();
//            }

//            // Obter Ids pelos nomes (robusto mesmo se já existirem)
//            var tipoAdminId = await ctx.TiposUsuario.Where(t => t.Descricao == "Administrador").Select(t => t.Id).FirstAsync();
//            var tipoGerenteId = await ctx.TiposUsuario.Where(t => t.Descricao == "Gerente").Select(t => t.Id).FirstAsync();
//            var tipoOutrosId = await ctx.TiposUsuario.Where(t => t.Descricao == "Outros").Select(t => t.Id).FirstAsync();

//            // =======================
//            // Roles (Identity)
//            // =======================
//            var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

//            async Task EnsureRoleAsync(string roleName)
//            {
//                if (!await roleMgr.RoleExistsAsync(roleName))
//                {
//                    var create = await roleMgr.CreateAsync(new IdentityRole<Guid>(roleName));
//                    if (!create.Succeeded)
//                    {
//                        var msg = string.Join("; ", create.Errors.Select(e => e.Description));
//                        throw new InvalidOperationException($"Falha ao criar role '{roleName}': {msg}");
//                    }
//                }
//            }

//            await EnsureRoleAsync("Admin");
//            await EnsureRoleAsync("Gerente");
//            await EnsureRoleAsync("Outros");

//            // =======================
//            // Usuários padrão (Admin, Gerente, Outros)
//            // =======================
//            var userMgr = sp.GetRequiredService<UserManager<ApplicationUser>>();

//            async Task<ApplicationUser> EnsureUserAsync(
//                string userName, string email, string password, string role, int tipoUsuarioId)
//            {
//                var user = await userMgr.FindByEmailAsync(email);
//                if (user is null)
//                {
//                    user = new ApplicationUser
//                    {
//                        Id = Guid.NewGuid(),
//                        UserName = userName,
//                        Email = email,
//                        EmailConfirmed = true,
//                        IsActive = true,
//                        TipoUsuarioId = tipoUsuarioId,
//                        CreatedAt = DateTime.UtcNow
//                    };

//                    var create = await userMgr.CreateAsync(user, password);
//                    if (!create.Succeeded)
//                    {
//                        var msg = string.Join("; ", create.Errors.Select(e => e.Description));
//                        throw new InvalidOperationException($"Falha ao criar usuário '{email}': {msg}");
//                    }
//                }
//                else
//                {
//                    // Garante status e tipo corretos se já existia
//                    if (!user.IsActive || user.TipoUsuarioId != tipoUsuarioId)
//                    {
//                        user.IsActive = true;
//                        user.TipoUsuarioId = tipoUsuarioId;
//                        user.UpdatedAt = DateTime.UtcNow;
//                        await ctx.SaveChangesAsync();
//                    }
//                }

//                if (!await userMgr.IsInRoleAsync(user, role))
//                {
//                    var add = await userMgr.AddToRoleAsync(user, role);
//                    if (!add.Succeeded)
//                    {
//                        var msg = string.Join("; ", add.Errors.Select(e => e.Description));
//                        throw new InvalidOperationException($"Falha ao atribuir role '{role}' ao usuário '{email}': {msg}");
//                    }
//                }

//                return user;
//            }

//            // Admin (já existia, mantido)
//            await EnsureUserAsync("admin", "admin@saphira.local", "Admin@123", "Admin", tipoAdminId);
//            // Gerente (novo usuário padrão)
//            await EnsureUserAsync("gerente", "gerente@saphira.local", "Gerente@123", "Gerente", tipoGerenteId);
//            // Outros (novo usuário padrão)
//            await EnsureUserAsync("usuario", "usuario@saphira.local", "Usuario@123", "Outros", tipoOutrosId);

//            // =======================
//            // Catálogos: Gêneros e Classificações
//            // =======================
//            if (!await ctx.Generos.AnyAsync())
//            {
//                ctx.Generos.AddRange(
//                    new Genero { Nome = "Slasher" },
//                    new Genero { Nome = "Sobrenatural" },
//                    new Genero { Nome = "Psicológico" },
//                    new Genero { Nome = "Found Footage" }
//                );
//                await ctx.SaveChangesAsync();
//            }

//            if (!await ctx.Classificacoes.AnyAsync())
//            {
//                ctx.Classificacoes.AddRange(
//                    new Classificacao { Nome = "12", Descricao = "Não recomendado para menores de 12" },
//                    new Classificacao { Nome = "14", Descricao = "Não recomendado para menores de 14" },
//                    new Classificacao { Nome = "16", Descricao = "Não recomendado para menores de 16" },
//                    new Classificacao { Nome = "18", Descricao = "Proibido para menores de 18" }
//                );
//                await ctx.SaveChangesAsync();
//            }

//            // =======================
//            // Filmes — inserir se não existir (comparando por Título)
//            // =======================
//            async Task EnsureFilmAsync(
//                string titulo, int ano, string generoNome, string classifNome, string imagemUrl, string? sinopse)
//            {
//                if (!await ctx.Filmes.AnyAsync(f => f.Titulo == titulo))
//                {
//                    var generoId = await ctx.Generos.Where(g => g.Nome == generoNome).Select(g => g.Id).FirstAsync();
//                    var classId = await ctx.Classificacoes.Where(c => c.Nome == classifNome).Select(c => c.Id).FirstAsync();
//                    ctx.Filmes.Add(new Filme
//                    {
//                        Titulo = titulo,
//                        Ano = ano,
//                        GeneroId = generoId,
//                        ClassificacaoId = classId,
//                        ImagemCapaUrl = imagemUrl,
//                        Sinopse = sinopse,
//                        CreatedAt = DateTime.UtcNow
//                    });
//                }
//            }

//            // Lote inicial (já existentes)
//            await EnsureFilmAsync("Sussurros da Meia-Noite", 2022, "Sobrenatural", "16",
//                "https://picsum.photos/seed/terror1/400/600", "Uma entidade ronda um vilarejo.");
//            await EnsureFilmAsync("A Faca do Lago", 1987, "Slasher", "18",
//                "https://picsum.photos/seed/terror2/400/600", "Um assassino mascarado retorna a cada verão.");
//            await EnsureFilmAsync("Câmera Perdida", 2011, "Found Footage", "16",
//                "https://picsum.photos/seed/terror3/400/600", "Gravações revelam algo que não deveria existir.");

//            // Extras já adicionados antes
//            await EnsureFilmAsync("O Sótão", 2009, "Psicológico", "14",
//                "https://picsum.photos/seed/terror4/400/600", "Segredos sombrios emergem de um velho casarão.");
//            await EnsureFilmAsync("Noite dos Ecos", 2018, "Sobrenatural", "16",
//                "https://picsum.photos/seed/terror5/400/600", "Ecos de vozes chamam por alguém que já se foi.");
//            await EnsureFilmAsync("Ritos Antigos", 1996, "Sobrenatural", "18",
//                "https://picsum.photos/seed/terror6/400/600", "Um ritual proibido reabre um portal ancestral.");
//            await EnsureFilmAsync("Labirinto das Sombras", 2020, "Psicológico", "16",
//                "https://picsum.photos/seed/terror7/400/600", "O que é real quando as paredes se movem?");
//            await EnsureFilmAsync("Marcas da Lâmina", 1984, "Slasher", "18",
//                "https://picsum.photos/seed/terror8/400/600", "A cidade tenta esquecer... ele não.");
//            await EnsureFilmAsync("Arquivo 404", 2013, "Found Footage", "16",
//                "https://picsum.photos/seed/terror9/400/600", "Fitas com trechos apagados que ninguém admite gravar.");

//            // --- SLASHER (até 8) ---
//            await EnsureFilmAsync("Noite da Lâmina", 1990, "Slasher", "18",
//                "https://picsum.photos/seed/terror10/400/600", "Uma sequência de crimes revela um padrão familiar.");
//            await EnsureFilmAsync("Máscara na Névoa", 2003, "Slasher", "16",
//                "https://picsum.photos/seed/terror11/400/600", "Um assassino usa a neblina como cobertura perfeita.");
//            await EnsureFilmAsync("Verão Sangrento", 1979, "Slasher", "18",
//                "https://picsum.photos/seed/terror12/400/600", "Férias no lago viram uma caçada mortal.");
//            await EnsureFilmAsync("Rua dos Sussurros", 1995, "Slasher", "16",
//                "https://picsum.photos/seed/terror13/400/600", "Gritos ecoam por becos onde ninguém vê nada.");
//            await EnsureFilmAsync("Carnaval Macabro", 2017, "Slasher", "18",
//                "https://picsum.photos/seed/terror14/400/600", "Entre fantasias, um rosto sem máscara caça à luz de neon.");
//            await EnsureFilmAsync("A Última Cabana", 1982, "Slasher", "16",
//                "https://picsum.photos/seed/terror15/400/600", "Um refúgio isolado com portas que não fecham por dentro.");

//            // --- SOBRENATURAL (até 8) ---
//            await EnsureFilmAsync("A Casa Entre Mundos", 2008, "Sobrenatural", "14",
//                "https://picsum.photos/seed/terror16/400/600", "Um cômodo aparece e some como se respirasse.");
//            await EnsureFilmAsync("Chamado do Além", 2015, "Sobrenatural", "16",
//                "https://picsum.photos/seed/terror17/400/600", "Telefonemas de números que não existem mais.");
//            await EnsureFilmAsync("Velas ao Vento", 1992, "Sobrenatural", "12",
//                "https://picsum.photos/seed/terror18/400/600", "Chamas tremulam formando nomes proibidos.");
//            await EnsureFilmAsync("O Sino da Meia-Noite", 2021, "Sobrenatural", "16",
//                "https://picsum.photos/seed/terror19/400/600", "Cada badalada convoca alguém do outro lado.");
//            await EnsureFilmAsync("Porta 13", 2001, "Sobrenatural", "18",
//                "https://picsum.photos/seed/terror20/400/600", "Uma entrada que leva a onde ninguém voltou.");

//            // --- PSICOLÓGICO (até 8) ---
//            await EnsureFilmAsync("Vozes na Parede", 2004, "Psicológico", "14",
//                "https://picsum.photos/seed/terror21/400/600", "Murmúrios que só ele escuta ditam o próximo passo.");
//            await EnsureFilmAsync("Quarto Sem Janelas", 1998, "Psicológico", "16",
//                "https://picsum.photos/seed/terror22/400/600", "Como fugir quando não há saída nem relógios?");
//            await EnsureFilmAsync("Metade de Mim", 2019, "Psicológico", "14",
//                "https://picsum.photos/seed/terror23/400/600", "Um doppelgänger começa a tomar decisões por ela.");
//            await EnsureFilmAsync("O Silêncio do Corredor", 1986, "Psicológico", "14",
//                "https://picsum.photos/seed/terror24/400/600", "O hospital deserto ainda chama pacientes pelo nome.");
//            await EnsureFilmAsync("Espelhos Partidos", 2013, "Psicológico", "16",
//                "https://picsum.photos/seed/terror25/400/600", "Reflexos mostram cenas que ainda não aconteceram.");
//            await EnsureFilmAsync("Ciclo da Madrugada", 2007, "Psicológico", "16",
//                "https://picsum.photos/seed/terror26/400/600", "3:33 reaparece todas as noites, com novas marcas.");

//            // --- FOUND FOOTAGE (até 8) ---
//            await EnsureFilmAsync("Fita 7B", 2002, "Found Footage", "16",
//                "https://picsum.photos/seed/terror27/400/600", "Gravação sem trilha: apenas passos e respirações.");
//            await EnsureFilmAsync("Malha Oculta", 2016, "Found Footage", "14",
//                "https://picsum.photos/seed/terror28/400/600", "Lives encadeadas revelam alguém observando a todos.");
//            await EnsureFilmAsync("Sinal Perdido", 2009, "Found Footage", "12",
//                "https://picsum.photos/seed/terror29/400/600", "Uma equipe segue coordenadas que mudam sozinhas.");
//            await EnsureFilmAsync("Arquivo Sem Origem", 2018, "Found Footage", "16",
//                "https://picsum.photos/seed/terror30/400/600", "Pastas surgem num HD recém-formatado.");
//            await EnsureFilmAsync("Três Dias Desaparecidos", 2011, "Found Footage", "16",
//                "https://picsum.photos/seed/terror31/400/600", "A filmadora voltou; eles não.");
//            await EnsureFilmAsync("No Escuro do Subsolo", 2020, "Found Footage", "18",
//                "https://picsum.photos/seed/terror32/400/600", "Exploradores urbanos ligam a luz… e algo pisca de volta.");

//            if (ctx.ChangeTracker.HasChanges())
//                await ctx.SaveChangesAsync();

//        }
//    }
//}



//refatorado fase 7

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaphiraTerror.Infrastructure.Entities; // ApplicationUser (Guid)
using SaphiraTerror.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace SaphiraTerror.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task EnsureSeededAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // Migrate first
        await db.Database.MigrateAsync();

        // 1) Roles (idempotente)
        await EnsureRoleAsync(roles, "Admin");
        await EnsureRoleAsync(roles, "Gerente");
        await EnsureRoleAsync(roles, "Outros");

        // 2) Usuários padrão (idempotente)
        await EnsureUserAsync(db, users, roles,
            email: "admin@saphira.local",
            userName: "admin",
            password: "Admin@123",
            roleName: "Admin");

        await EnsureUserAsync(db, users, roles,
            email: "gerente@saphira.local",
            userName: "gerente",
            password: "Gerente@123",
            roleName: "Gerente");

        await EnsureUserAsync(db, users, roles,
            email: "usuario@saphira.local",
            userName: "usuario",
            password: "Usuario@123",
            roleName: "Outros");

        // 3) Demais seeds do domínio (gêneros, classificações, filmes etc.)
        await SeedDomainAsync(db);
    }

    // ---------- helpers ----------

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole<Guid>> roles, string name)
    {
        if (!await roles.RoleExistsAsync(name))
        {
            await roles.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = name, NormalizedName = name.ToUpperInvariant() });
        }
    }

    private static async Task EnsureUserAsync(
        AppDbContext db,
        UserManager<ApplicationUser> users,
        RoleManager<IdentityRole<Guid>> roles,
        string email,
        string userName,
        string password,
        string roleName)
    {
        var normalizedEmail = email.ToUpperInvariant();

        // Ignora query filters para também encontrar inativos
        var existing = await db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

        ApplicationUser user;

        if (existing is null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                NormalizedEmail = normalizedEmail,
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var res = await users.CreateAsync(user, password);
            if (!res.Succeeded)
            {
                // se falhar por qualquer motivo, não derruba a app
                // (mas é útil logar isso em produção)
                return;
            }
        }
        else
        {
            user = existing;

            // normaliza valores faltantes (sem quebrar)
            var fixedUp = false;
            if (string.IsNullOrWhiteSpace(user.NormalizedEmail))
            {
                user.NormalizedEmail = user.Email?.ToUpperInvariant();
                fixedUp = true;
            }
            if (string.IsNullOrWhiteSpace(user.NormalizedUserName) && !string.IsNullOrWhiteSpace(user.UserName))
            {
                user.NormalizedUserName = user.UserName.ToUpperInvariant();
                fixedUp = true;
            }
            if (fixedUp)
            {
                db.Users.Update(user);
                await db.SaveChangesAsync();
            }
        }

        // Garante a role
        //var realRole = await roles.Roles
        //    .Where(r => r.Name != null)
        //    .Select(r => r.Name!)
        //    .FirstOrDefaultAsync(n => string.Equals(n, roleName, StringComparison.OrdinalIgnoreCase));

        var normalized = roleName.ToUpperInvariant();
        var roleEntity = await roles.Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalized);
        var realRole = roleEntity?.Name;


        if (realRole is null) return;

        var inRole = await users.IsInRoleAsync(user, realRole);
        if (!inRole)
        {
            await users.AddToRoleAsync(user, realRole);
        }
    }

    private static async Task SeedDomainAsync(AppDbContext db)
    {
        // coloque aqui seus seeds de gênero, classificação, filmes etc.
        // SEM inserir duplicados. Exemplo:
        // if (!await db.Generos.AnyAsync()) { db.Generos.AddRange(...); await db.SaveChangesAsync(); }
    }
}
