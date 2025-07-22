using RouteSearchApi.Application.Contracts.Request;

namespace RouteSearchApi.Domain.Models;

public class RouteModel
{
    public RouteModel() { }

    public RouteModel(string origem, string destino, decimal valor)
    {
        Origem = origem;
        Destino = destino;
        Valor = valor;
    }

    public RouteModel(RouteInsertRequestDto requestDto)
    {
        Origem = requestDto.Origem;
        Destino = requestDto.Destino;
        Valor = requestDto.Valor;
    }

    public int Id { get; private set; }

    public string Origem { get; private set; } = string.Empty;

    public string Destino { get; private set; } = string.Empty;

    public decimal Valor { get; private set; }

    public void Update(RouteUpdateRequestDto requestDto)
    {
        Origem = requestDto.Origem;
        Destino = requestDto.Destino;
        Valor = requestDto.Valor;
    }
}
