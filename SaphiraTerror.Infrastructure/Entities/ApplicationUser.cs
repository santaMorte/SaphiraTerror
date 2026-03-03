using Microsoft.AspNetCore.Identity;

namespace SaphiraTerror.Infrastructure.Entities;

/// <summary>
/// Usuário Identity com campos de domínio essenciais.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public bool IsActive { get; set; } = true;      // soft delete
    public int TipoUsuarioId { get; set; }          // FK para TipoUsuario
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
