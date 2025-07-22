using RouteSearchApi.Domain.Models;

namespace RouteSearchApi.Application.Contracts.Response;

public class RouteResponseDto
{
    public RouteResponseDto(RouteModel model)
    {
        Id = model.Id;
        Origem = model.Origem;
        Destino = model.Destino;
        Valor = model.Valor;
    }

    public int Id { get; private set; }

    public string Origem { get; private set; } = string.Empty;

    public string Destino { get; private set; } = string.Empty;

    public decimal Valor { get; private set; }
}
