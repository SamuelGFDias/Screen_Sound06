using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.API.Endpoints;

public static class Musicas
{
    private static ICollection<MusicaResponse> EntityListToResponseList(IEnumerable<Musica> musicaList)
    {
        return musicaList.Select(EntityToResponse).ToList();
    }

    private static MusicaResponse EntityToResponse(Musica musica)
    {
        return new MusicaResponse(musica.Id, musica.Nome, musica.Artista!.Id, musica.Artista.Nome);
    }

    public static void AddEndpointsMusicas(this WebApplication app)
    {
        app.MapGet("/Musicas", ([FromServices] DAL<Musica> dal) =>
        {
            return EntityListToResponseList(dal.Listar());
        });
        app.MapGet("/Musicas/{nome}", ([FromServices] DAL<Musica> dal, string nome) =>
        {
            var musicaRecuperada = dal.RecuperarPor(m => m.Nome.Equals(nome));
            if (musicaRecuperada is null) return Results.NotFound();
            return Results.Ok(EntityToResponse(musicaRecuperada));
        });
        app.MapPost("/Musicas", ([FromServices] DAL<Musica> dal, [FromServices] DAL<Genero> dalGenero, [FromBody] MusicaRequest musicaRequest) =>
        {
            var musica = new Musica(musicaRequest.Nome)
            {
                ArtistaId = musicaRequest.ArtistaId,
                AnoLancamento = musicaRequest.AnoLancamento,
                Generos = GeneroRequestConverter(musicaRequest.Generos, dalGenero)
            };
            dal.Adicionar(musica);
            return Results.Ok();
        });
        app.MapDelete("/Musicas/{id}", ([FromServices] DAL<Musica> dal, int id) =>
        {
            var musicaRecuperada = dal.RecuperarPor(m => m.Id.Equals(id));
            if (musicaRecuperada is null) return Results.NotFound();
            dal.Deletar(musicaRecuperada);
            return Results.NoContent();
        });
        app.MapPut("/Musicas", ([FromServices] DAL<Musica> dal, [FromBody] MusicaRequestEdit musicaRequestEdit) =>
        {
            var musicaAtualizada = dal.RecuperarPor(m => m.Id.Equals(musicaRequestEdit.Id));
            if (musicaAtualizada is null) return Results.NotFound();

            musicaAtualizada.Nome = musicaRequestEdit.Nome;
            musicaAtualizada.AnoLancamento = musicaRequestEdit.AnoLancamento;
            dal.Atualizar(musicaAtualizada);
            return Results.Ok();
        });
    }

    private static ICollection<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generos, DAL<Genero> dalGenero)
    {
        var listaGeneros = new List<Genero>();
        foreach (var genero in generos)
        {
            var entity = RequestToEntity(genero);
            var generoRecuperado = dalGenero.RecuperarPor(g => g.Nome.ToUpper().Equals(entity.Nome.ToUpper()));
            if (generoRecuperado is null)
            {
                listaGeneros.Add(entity);
            }
            else
            {
                listaGeneros.Add(generoRecuperado);
            }
        }
        return listaGeneros;
    }
    private static Genero RequestToEntity(GeneroRequest generoRequest)
    {
        return new Genero()
        {
            Nome = generoRequest.Nome,
            Descricao = generoRequest.Descricao
        };
    }
}
