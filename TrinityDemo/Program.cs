using AbanoubNassem.Trinity.Extensions;
using AbanoubNassem.Trinity.Models;
using Microsoft.Data.Sqlite;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTrinity(configs =>
{
    configs.ConnectionFactory = () => new SqliteConnection("Data Source=sakila;");
    configs.AuthenticateUser = (_, email, _) =>
    {
        var isAdmin = email == "admin@admin.com";
        return Task.FromResult(
            new TrinityUser(
                "123456", //identifier
                isAdmin ? "Administrator" : email.Split('@').First(), //name
                email, // email
                isAdmin ? "admin" : "user", // role
                "https://i.pravatar.cc/300" // avatar
            )
        )!;
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseTrinity();
app.Run();