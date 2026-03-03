namespace SaphiraTerror.Domain.Entities;

public class TipoUsuario
{
    public int Id { get; set; } // 1=Admin, 2=Gerente, 3=Outros
    public string Descricao { get; set; } = default!;
}
