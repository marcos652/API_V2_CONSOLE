// File: Web.Api/Controllers/ClientsController.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Ports.Input;
using Web.Api.DTOs;
using Core.Domain.Entities; // Necessário para DTO -> Entity mapping

// [Authorize] // Adicione isso após implementar a autenticação JWT
[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    /// <summary>
    /// Obtém lista de clientes com paginação e filtros (Clientes.html).
    /// Endpoint: GET /api/clients?pageNumber=1&pageSize=10
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResponseDTO<ClientDTO>>> GetClientsPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string searchName = null,
        [FromQuery] string status = null)
    {
        var pagedClients = await _clientService.GetClientsWithPaginationAsync(
            pageNumber,
            pageSize,
            searchName,
            status
        );

        // Mapeia o PagedList da Entidade para o DTO de Resposta da API
        var response = PagedResponseDTO<ClientDTO>.FromPagedList(pagedClients);

        return Ok(response);
    }

    // Método de exemplo: POST para adicionar novo cliente
    [HttpPost]
    public async Task<ActionResult<ClientDTO>> AddClient([FromBody] ClientCreateDTO dto)
    {
        // **TODO: Mapear ClientCreateDTO para Core.Domain.Entities.Client**
        var newClient = /* ... mapeamento ... */;

        var createdClient = await _clientService.CreateClientAsync(newClient);

        // **TODO: Mapear Core.Domain.Entities.Client para ClientDTO**
        var clientDto = /* ... mapeamento ... */;

        return CreatedAtAction(nameof(GetClientsPaged), new { id = createdClient.Id }, clientDto);
    }

    // Você adicionaria aqui: GET by ID, PUT (Update) e DELETE
}