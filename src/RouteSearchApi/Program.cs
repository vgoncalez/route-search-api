using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RouteSearchApi.Application.Interfaces.Services;
using RouteSearchApi.Application.Services;
using RouteSearchApi.Domain.Models;
using RouteSearchApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("RouteSearchDb"));
builder.Services.AddSwaggerGen(options =>
{
    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddScoped<IRouteService, RouteService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    db.Database.EnsureCreated();

    if (!db.Rotas.Any())
    {
        db.Rotas.AddRange(
            new RouteModel("GRU", "BRC", 10),
            new RouteModel("BRC", "SCL", 5),
            new RouteModel("GRU", "CDG", 75),
            new RouteModel("GRU", "SCL", 20),
            new RouteModel("GRU", "ORL", 56),
            new RouteModel("ORL", "CDG", 5),
            new RouteModel("SCL", "ORL", 20)
        );
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
