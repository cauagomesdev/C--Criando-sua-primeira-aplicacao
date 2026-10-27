using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UsuariosApi.Authorization;
using UsuariosApi.Data;
using UsuariosApi.Models;
using UsuariosApi.Services;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("UsuarioConnection");

// Trava com erro explícito se a string não for encontrada no Secrets
if (string.IsNullOrEmpty(connString))
{
    throw new InvalidOperationException(
        "A string de conexão 'UsuarioConnection' não foi encontrada. Verifique o User Secrets!"
    );
}

builder.Services.AddDbContext<UsuarioDbContext>(opts =>
    opts.UseMySql(connString, new MySqlServerVersion(new Version(8, 0, 30)))
);

builder
    .Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<UsuarioDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["SymmetricSecurityKey"])
                ),
                ValidateAudience = false,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero,
            };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IdadeMinima", policy => policy.AddRequirements(new IdadeMinima(18)));
});

builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddSingleton<IAuthorizationHandler, IdadeAuthorization>();

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers(); // Ou suas Minimal APIs (app.MapGet...)

app.Run();
