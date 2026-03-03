namespace SaphiraTerror.Domain.Entities;

public class Classificacao
{
    public int Id { get; set; }
    public string Nome { get; set; } = default!;   // Ex.: "16", "18"
    public string? Descricao { get; set; }         // Opcional
    public ICollection<Filme> Filmes { get; set; } = new List<Filme>();
}
