using RouteSearchApi.Application.Contracts.Request;
using RouteSearchApi.Application.Contracts.Response;

namespace RouteSearchApi.Application.Interfaces.Services;

public interface IRouteService
{
    Task<List<RouteResponseDto>> GetAllAsync();

    Task<RouteResponseDto> InsertAsync(RouteInsertRequestDto requestDto);

    Task<RouteResponseDto> UpdateAsync(RouteUpdateRequestDto requestDto);

    Task<bool> DeleteAsync(int id);

    Task<string?> GetBestAsync(RouteBestRequestDto requestDto);
}
