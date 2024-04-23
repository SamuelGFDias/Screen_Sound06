using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.API.Response;

public record MusicaResponse(int Id, string Nome, int ArtistaId, string NomeArtista);
