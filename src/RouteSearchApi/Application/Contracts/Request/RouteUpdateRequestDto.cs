using System.ComponentModel.DataAnnotations;

namespace RouteSearchApi.Application.Contracts.Request;

public class RouteUpdateRequestDto
{
    public required int Id { get; set; }

    public required string Origem { get; set; }

    public required string Destino { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public required decimal Valor { get; set; }
}
