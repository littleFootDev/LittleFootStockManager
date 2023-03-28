using LittleFootStockManager.Configuration;
using LittleFootStockManager.Data;
using LittleFootStockManager.Data.Model;
using LittleFootStockManager.Endpoint;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("LittleFootStockManagerDbConnectionString");
builder.Services.AddDbContext<LittleFootStockManagerDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddIdentity<Users, IdentityRole>()
    .AddEntityFrameworkStores<LittleFootStockManagerDbContext>()
    .AddDefaultTokenProviders();
RSA _rsa = RSA.Create();
if (!File.Exists("Key.bin"))
{
    var key = _rsa.ExportRSAPrivateKey();
    File.WriteAllBytes("key.bin", key);
}
_rsa.ImportRSAPrivateKey(File.ReadAllBytes("key.bin"), out var _);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new RsaSecurityKey(_rsa)
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b =>
                b.AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod()
    );
});

builder.Host.UseSerilog((ctx, LoggerConfiguration) =>
{
    LoggerConfiguration.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddAuthManagerService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/auth").MapAuthManagerEndpoint();
app.Run();

