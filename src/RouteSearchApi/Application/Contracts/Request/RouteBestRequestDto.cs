namespace RouteSearchApi.Application.Contracts.Request;

public class RouteBestRequestDto
{
    public required string Origem { get; set; }

    public required string Destino { get; set; }
}
