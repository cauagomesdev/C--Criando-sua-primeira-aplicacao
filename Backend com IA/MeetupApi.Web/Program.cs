using MeetupApi.Web.Data;
using MeetupApi.Web.Dtos;
using MeetupApi.Web.Middlewares;
using MeetupApi.Web.Repositories;
using MeetupApi.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Validation errors from [ApiController] return 422 with { "mensagem": "..." }
        options.InvalidModelStateResponseFactory = context =>
        {
            var mensagem = string.Join(", ", context.ModelState
                .Where(entry => entry.Value is { Errors.Count: > 0 })
                .SelectMany(entry => entry.Value!.Errors.Select(error =>
                    $"{entry.Key}: {(string.IsNullOrEmpty(error.ErrorMessage) ? "valor inválido" : error.ErrorMessage)}")));
            return new UnprocessableEntityObjectResult(new ErrorResponse(mensagem));
        };
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MeetupDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.36-mysql")));

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();

builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<ParticipantService>();
builder.Services.AddScoped<RegistrationService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MeetupDbContext>();
    db.Database.EnsureCreated();
}

app.Run();

public partial class Program { }
