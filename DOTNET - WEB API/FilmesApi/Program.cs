var builder = WebApplication.CreateBuilder(args);

// 1. Registra o suporte aos Controllers no container
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// 2. Mapeia automaticamente todas as classes dentro da pasta Controllers
app.MapControllers();

app.Run();
