using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaphiraTerror.Application.Abstractions.Repositories;
using SaphiraTerror.Application.Services;
using SaphiraTerror.Infrastructure.Entities;
using SaphiraTerror.Infrastructure.Persistence;
using SaphiraTerror.Infrastructure.Repositories;
using SaphiraTerror.Infrastructure.Services;

namespace SaphiraTerror.Infrastructure;

public static class DependencyInjection
{
    //criado na fase 1
    //public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    //{
    //    var conn = config.GetConnectionString("DefaultConnection")
    //               ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' não configurada.");

    //    services.AddDbContext<AppDbContext>(opt =>
    //        opt.UseSqlServer(conn));

    //    services
    //        .AddIdentityCore<ApplicationUser>(o =>
    //        {
    //            o.User.RequireUniqueEmail = true;
    //            o.Password.RequiredLength = 6;
    //            o.Password.RequireNonAlphanumeric = false;
    //            o.Password.RequireUppercase = false;
    //            o.Password.RequireDigit = false;
    //        })
    //        .AddRoles<IdentityRole<Guid>>()
    //        .AddEntityFrameworkStores<AppDbContext>();

    //    return services;
    //}


    //1ª refatoracao -  fase 2

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' não configurada.");

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(conn));

        services
            .AddIdentityCore<ApplicationUser>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequiredLength = 6;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireDigit = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        // Repositórios
        services.AddScoped<IFilmeRepository, EfFilmeRepository>();
        services.AddScoped<IGeneroRepository, EfGeneroRepository>();
        services.AddScoped<IClassificacaoRepository, EfClassificacaoRepository>();

        // Serviços de aplicação
        services.AddScoped<IFilmeQueryService, FilmeQueryService>();
        services.AddScoped<ICatalogLookupService, CatalogLookupService>();

        return services;
    }
}


