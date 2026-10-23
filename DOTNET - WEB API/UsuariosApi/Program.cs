var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Se for usar Controllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Gerador do Swagger

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Habilita a interface gráfica na rota /swagger
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); // Ou suas Minimal APIs (app.MapGet...)

app.Run();
