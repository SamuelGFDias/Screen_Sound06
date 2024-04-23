using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Modelos;

namespace ScreenSound.API.Endpoints;

public static class ArtistasExtensions
{
    private static ICollection<ArtistaResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
    {
        return listaDeArtistas.Select(EntityToResponse).ToList();
    }
    private static ArtistaResponse EntityToResponse(Artista artista)
    {
        return new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil);
    }


    public static void AddEndpointsArtistas(this WebApplication app)
    {
        app.MapGet("/Artistas", ([FromServices] DAL<Artista> dal) =>
        {
            return EntityListToResponseList(dal.Listar());
        });
        app.MapGet("/Artistas/{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
        {
            var artistaRecuperado = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
            if (artistaRecuperado is null) return Results.NotFound();
            return Results.Ok(EntityToResponse(artistaRecuperado));
        });
        app.MapPost("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
        {
            var artista = new Artista(artistaRequest.Nome, artistaRequest.Bio);
            dal.Adicionar(artista);
            return Results.Ok();
        });
        app.MapDelete("/Artistas/{id}", ([FromServices] DAL<Artista> dal, int id) =>
        {
            var artistaRecuperado = dal.RecuperarPor(a => a.Id.Equals(id));
            if (artistaRecuperado is null) return Results.NotFound();
            dal.Deletar(artistaRecuperado);
            return Results.NoContent();
        });
        app.MapPut("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequetsEdit artistaRequestEdit) =>
        {
            var artistaAtualizado = dal.RecuperarPor(a => a.Id.Equals(artistaRequestEdit.Id));
            if (artistaAtualizado is null) return Results.NotFound();
            artistaAtualizado.Nome = artistaRequestEdit.Nome;
            artistaAtualizado.Bio = artistaRequestEdit.Bio;

            dal.Atualizar(artistaAtualizado);
            return Results.Ok();
        });
    }
}
