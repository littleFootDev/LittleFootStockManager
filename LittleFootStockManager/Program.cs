using LittleFootStockManager.Configuration;
using LittleFootStockManager.Data;
using LittleFootStockManager.Data.Model;
using LittleFootStockManager.Endpoint;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

builder.Services.AddAuthentication();
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

