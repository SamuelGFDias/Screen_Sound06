using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.API.Endpoints;

public static class GeneroExtensions
{
    private static ICollection<GeneroResponse> EntityListToRequestList(IEnumerable<Genero> generos)
    {
        return generos.Select(EntityToRequest).ToList();
    }
    private static GeneroResponse EntityToRequest(Genero genero)
    {
        return new GeneroResponse(genero.Id, genero.Nome, genero.Descricao);
    }

    public static void AddEndpointsGeneros(this WebApplication app)
    {
        app.MapGet("/Generos", ([FromServices] DAL<Genero> dal) =>
        {
            return EntityListToRequestList(dal.Listar());
        });
        app.MapGet("/Generos/{nome}", ([FromServices] DAL<Genero> dal, string nome) =>
        {
            var generoRecuperado = dal.RecuperarPor(g => g.Nome.Equals(nome));
            if (generoRecuperado is null) return Results.NotFound();
            return Results.Ok(EntityToRequest(generoRecuperado));
        });
        app.MapPost("/Generos", ([FromServices] DAL<Genero> dal, [FromBody] GeneroRequest generoRequest) =>
        {
            var genero = new Genero()
            {
                Nome = generoRequest.Nome, 
                Descricao = generoRequest.Descricao
            };
            dal.Adicionar(genero);
            return Results.Ok();
        });
        app.MapDelete("/Generos/{id}", ([FromServices] DAL<Genero> dal, int id) =>
        {
            var generoRecuperado = dal.RecuperarPor(g => g.Id.Equals(id));
            if (generoRecuperado is null) return Results.NotFound();
            dal.Deletar(generoRecuperado);
            return Results.NoContent();
        });
        app.MapPut("/Generos", ([FromServices] DAL<Genero> dal, [FromBody] GeneroRequestEdit generoRequestEdit) =>
        {
            var generoAtualizado = dal.RecuperarPor(g => g.Id.Equals(generoRequestEdit.Id));
            if (generoAtualizado is null) return Results.NotFound();

            generoAtualizado.Nome = generoRequestEdit.Nome;
            generoAtualizado.Descricao = generoRequestEdit.Descricao;
            dal.Atualizar(generoAtualizado);
            return Results.Ok();
        });
    }
}
