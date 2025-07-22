using Microsoft.EntityFrameworkCore;
using RouteSearchApi.Application.Contracts.Request;
using RouteSearchApi.Application.Services;
using RouteSearchApi.Domain.Models;
using RouteSearchApi.Infrastructure.Data;
using Xunit;

namespace RouteSearchApi.Tests.Services;

public class RouteServiceTests
{
    private ApplicationDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RouteServiceTests")
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }

    private void SeedRoutes(ApplicationDbContext context)
    {
        context.Rotas.AddRange(
            new RouteModel("GRU", "BRC", 10),
            new RouteModel("BRC", "SCL", 5),
            new RouteModel("GRU", "CDG", 75),
            new RouteModel("GRU", "SCL", 20),
            new RouteModel("GRU", "ORL", 56),
            new RouteModel("ORL", "CDG", 5),
            new RouteModel("SCL", "ORL", 20)
         );

        context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Routes()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Rotas.Add(new RouteModel("A", "B", 10));
        context.Rotas.Add(new RouteModel("B", "C", 15));
        await context.SaveChangesAsync();
        var service = new RouteService(context);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task InsertAsync_Should_Add_Route()
    {
        // Arrange
        var context = GetInMemoryContext();
        var service = new RouteService(context);
        var request = new RouteInsertRequestDto { Origem = "GRU", Destino = "CDG", Valor = 10 };

        // Act
        var result = await service.InsertAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("GRU", result.Origem);
        Assert.Equal("CDG", result.Destino);
        Assert.Equal(10, result.Valor);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyRoute()
    {
        var context = GetInMemoryContext();
        var route = new RouteModel("GRU", "BRC", 10);
        context.Rotas.Add(route);
        await context.SaveChangesAsync();

        var service = new RouteService(context);

        var updateDto = new RouteUpdateRequestDto
        {
            Id = route.Id,
            Origem = "GRU",
            Destino = "BRC",
            Valor = 15
        };

        var updated = await service.UpdateAsync(updateDto);

        Assert.Equal(15, updated.Valor);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Route()
    {
        // Arrange
        var context = GetInMemoryContext();
        var route = new RouteModel("X", "Y", 30);
        context.Rotas.Add(route);
        await context.SaveChangesAsync();
        var service = new RouteService(context);

        // Act
        var success = await service.DeleteAsync(route.Id);

        // Assert
        Assert.True(success);
        Assert.Empty(context.Rotas);
    }

    [Fact]
    public async Task GetBestAsync_Should_Return_Shortest_Path()
    {
        // Arrange
        var context = GetInMemoryContext();
        context.Rotas.AddRange(new List<RouteModel>
        {
            new RouteModel("GRU", "BRC", 10),
            new RouteModel ( "BRC", "SCL", 5 ),
            new RouteModel ( "SCL", "CDG", 15),
            new RouteModel ( "GRU", "CDG", 50),
        });

        await context.SaveChangesAsync();
        var service = new RouteService(context);

        var dto = new RouteBestRequestDto { Origem = "GRU", Destino = "CDG" };

        // Act
        var result = await service.GetBestAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("GRU - BRC - SCL - CDG", result);
        Assert.Contains("30", result);
    }

    [Fact]
    public async Task GetBestAsync_ShouldReturnCheapestRoute()
    {
        // Arrange
        var context = GetInMemoryContext();
        SeedRoutes(context);
        var service = new RouteService(context);

        var request = new RouteBestRequestDto
        {
            Origem = "GRU",
            Destino = "CDG"
        };

        // Act
        var result = await service.GetBestAsync(request);

        // Assert
        Assert.Equal("GRU - BRC - SCL - ORL - CDG ao custo de 40,00", result);
    }

    [Fact]
    public async Task GetBestAsync_ShouldReturnError_WhenOriginEqualsDestination()
    {
        var context = GetInMemoryContext();
        SeedRoutes(context);
        var service = new RouteService(context);

        var request = new RouteBestRequestDto
        {
            Origem = "GRU",
            Destino = "GRU"
        };

        var result = await service.GetBestAsync(request);

        Assert.Equal("Origem e destino não podem ser iguais.", result);
    }

    [Fact]
    public async Task GetBestAsync_ShouldReturnError_WhenNoPathExists()
    {
        var context = GetInMemoryContext();
        SeedRoutes(context);
        var service = new RouteService(context);

        var request = new RouteBestRequestDto
        {
            Origem = "SCL",
            Destino = "XXX"
        };

        var result = await service.GetBestAsync(request);

        Assert.Equal("Nenhuma rota disponível entre os pontos informados.", result);
    }
}
