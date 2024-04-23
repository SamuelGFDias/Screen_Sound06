using ScreenSound.Modelos;

namespace ScreenSound.Shared.Modelos.Modelos;
public class Genero
{
    public virtual ICollection<Musica> Musicas { get; set; }
    public int Id { get; set; }
    public string? Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Nome: {Nome} - Descrição: {Descricao}";
    }
}
