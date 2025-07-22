using System.Globalization;
using Microsoft.EntityFrameworkCore;
using RouteSearchApi.Application.Contracts.Request;
using RouteSearchApi.Application.Contracts.Response;
using RouteSearchApi.Application.Interfaces.Services;
using RouteSearchApi.Domain.Models;
using RouteSearchApi.Infrastructure.Data;

namespace RouteSearchApi.Application.Services;

public class RouteService : IRouteService
{
    private readonly ApplicationDbContext _context;

    public RouteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RouteResponseDto>> GetAllAsync()
    {
        var routes = await _context.Rotas.AsNoTracking().ToListAsync();

        if (!routes.Any())
            return new List<RouteResponseDto>();

        var response = routes
            .Select(x => new RouteResponseDto(x))
            .ToList();

        return response;
    }

    public async Task<RouteResponseDto> InsertAsync(RouteInsertRequestDto requestDto)
    {
        var route = new RouteModel(requestDto);

        _context.Rotas.Add(route);
        await _context.SaveChangesAsync();

        return new RouteResponseDto(route);
    }

    public async Task<RouteResponseDto> UpdateAsync(RouteUpdateRequestDto requestDto)
    {
        var route = await _context.Rotas.FindAsync(requestDto.Id);
        if (route == null)
            throw new Exception($"Rota com ID {requestDto.Id} não encontrada.");

        route.Update(requestDto);

        await _context.SaveChangesAsync();

        return new RouteResponseDto(route);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var route = await _context.Rotas.FindAsync(id);
        if (route == null)
            return false;

        _context.Rotas.Remove(route);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<string?> GetBestAsync(RouteBestRequestDto requestDto)
    {
        if (requestDto.Origem == requestDto.Destino)
            return "Origem e destino não podem ser iguais.";

        var allSegments = await _context.Rotas.ToListAsync();

        var routeOptions = new List<(List<string> Path, decimal TotalCost)>();

        ExplorePaths(
            allSegments,
            current: requestDto.Origem,
            target: requestDto.Destino,
            pathSoFar: new List<string>(),
            costSoFar: 0,
            collectedPaths: routeOptions
        );

        if (!routeOptions.Any())
            return "Nenhuma rota disponível entre os pontos informados.";

        var cheapest = routeOptions.OrderBy(r => r.TotalCost).First();

        return $"{string.Join(" - ", cheapest.Path)} ao custo de {cheapest.TotalCost.ToString("C0", CultureInfo.CreateSpecificCulture("en-US"))}";
    }

    private void ExplorePaths(
        List<RouteModel> segments,
        string current,
        string target,
        List<string> pathSoFar,
        decimal costSoFar,
        List<(List<string> Path, decimal TotalCost)> collectedPaths)
    {
        if (pathSoFar.Contains(current))
            return;

        pathSoFar.Add(current);

        if (current == target)
        {
            collectedPaths.Add((new List<string>(pathSoFar), costSoFar));
            pathSoFar.RemoveAt(pathSoFar.Count - 1);
            return;
        }

        var nextSegments = segments
            .Where(s => s.Origem == current)
            .ToList();

        foreach (var segment in nextSegments)
        {
            ExplorePaths(
                segments,
                current: segment.Destino,
                target: target,
                pathSoFar: pathSoFar,
                costSoFar: costSoFar + segment.Valor,
                collectedPaths: collectedPaths
            );
        }

        pathSoFar.RemoveAt(pathSoFar.Count - 1);
    }
}
