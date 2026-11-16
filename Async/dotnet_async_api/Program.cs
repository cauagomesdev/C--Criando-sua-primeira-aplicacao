using dotnet_async_api.Context;
using dotnet_async_api.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<JornadaMilhasContext>(options => options.UseInMemoryDatabase("JornadaMilhasVoos"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<JornadaMilhasContext>();

    // Carga de dados inicial
    context.Voos.AddRange(
        new Voo { Id = 1, Origem = "Brasilia", Destino = "Recife", Preco = 2000, MilhasNecessarias = 10000 },
            new Voo { Id = 2, Origem = "Vitória", Destino = "São Paulo", Preco = 2500, MilhasNecessarias = 15000 },
            new Voo { Id = 3, Origem = "Salvador", Destino = "Florianópolis", Preco = 3000, MilhasNecessarias = 20000 }
    );
    context.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/Hello", () => "Hello World! - API online.").WithTags("Voos").WithSummary("Verificação do status 'Online'").WithOpenApi();

app.MapGet("/voos", async ([FromServices]JornadaMilhasContext context) => { 

    Task.Delay(5000).Wait();
    return await context.Voos.ToListAsync();

}).WithTags("Voos").WithSummary("Lista os vôos cadastrados.").WithOpenApi();

app.MapPost("/voos/comprar",async([FromServices]JornadaMilhasContext context, [FromBody] CompraPassagemRequest request) =>
{
    var mensagemCompra = $"Passagem comprada origem: {request.Origem} com destino: {request.Destino} com milhas {request.Milhas}";
    return Results.Ok(await Task.FromResult(mensagemCompra));

}).WithTags("Voos").WithSummary("Simula a compra de passagem").WithOpenApi();

app.Run();