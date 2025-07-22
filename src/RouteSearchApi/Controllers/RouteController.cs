using Microsoft.AspNetCore.Mvc;
using RouteSearchApi.Application.Contracts.Request;
using RouteSearchApi.Application.Contracts.Response;
using RouteSearchApi.Application.Interfaces.Services;

namespace RouteSearchApi.Controllers;

/// <summary>
/// Controlador responsável pelas operações CRUD de rotas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly IRouteService _routeService;

    public RouteController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    /// <summary>
    /// Obtém todas as rotas cadastradas.
    /// </summary>
    /// <returns>Lista de rotas.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RouteResponseDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync()
    {
        var response = await this._routeService.GetAllAsync();

        return Ok(response);
    }

    /// <summary>
    /// Cadastra uma nova rota.
    /// </summary>
    /// <param name="requestDto">Dados da rota a ser cadastrada.</param>
    /// <returns>Resultado da operação.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InsertAsync([FromBody] RouteInsertRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _routeService.InsertAsync(requestDto);
        return Ok(response);
    }

    /// <summary>
    /// Atualiza uma rota existente.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestDto">Dados atualizados da rota.</param>
    /// <returns>Resultado da operação.</returns>
    [HttpPut]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] RouteUpdateRequestDto requestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        requestDto.Id = id;

        var response = await _routeService.UpdateAsync(requestDto);
        return Ok(response);
    }

    /// <summary>
    /// Remove uma rota pelo Id.
    /// </summary>
    /// <param name="id">Identificador da rota.</param>
    /// <returns>Resultado da operação.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var success = await _routeService.DeleteAsync(id);

        if (!success)
            return NotFound($"Rota com ID {id} não encontrada.");

        return Ok($"Rota com ID {id} removida com sucesso.");
    }

    /// <summary>
    /// Consulta a melhor rota (mais barata) entre dois aeroportos.
    /// </summary>
    /// <param name="requestDto">Objeto contendo os códigos IATA da origem e destino.</param>
    /// <returns>
    /// A melhor rota no formato "GRU - BRC - SCL - ORL - CDG ao custo de $40", 
    /// ou erro caso não seja possível encontrar.
    /// </returns>
    /// <response code="200">Retorna a melhor rota encontrada com menor custo.</response>
    /// <response code="400">Se os parâmetros de origem ou destino não forem informados.</response>
    /// <response code="404">Se nenhuma rota possível for encontrada entre origem e destino.</response>
    /// <response code="500">Erro interno do servidor.</response>
    [HttpGet("best")]
    [ProducesResponseType(200, Type = typeof(string))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetBestRouteAsync([FromQuery] RouteBestRequestDto requestDto)
    {
        if (string.IsNullOrEmpty(requestDto.Origem) || string.IsNullOrEmpty(requestDto.Destino))
            return BadRequest("Origem e destino são obrigatórios.");

        var resultado = await _routeService.GetBestAsync(requestDto);

        if (resultado == null)
            return NotFound($"Não foi encontrada uma rota entre {requestDto.Origem} e {requestDto.Destino}.");

        return Ok(resultado);
    }
}
