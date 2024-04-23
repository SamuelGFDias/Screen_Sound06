using System.ComponentModel.DataAnnotations;

namespace ScreenSound.API.Requests;

public record ArtistaRequetsEdit(int Id, [Required] string Nome, [Required] string Bio) 
    : ArtistaRequest(Nome, Bio);
