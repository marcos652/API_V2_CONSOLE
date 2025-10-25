// File: Web.Api/Controllers/ClientFormController.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Core.Application.Ports.Input;

[ApiController]
[Route("api/external/form")]
public class ClientFormController : ControllerBase
{
    private readonly IClientFormService _formService;

    public ClientFormController(IClientFormService formService)
    {
        _formService = formService;
    }

    /// <summary>
    /// Recebe e armazena os dados brutos submetidos pelo cliente.
    /// Endpoint: POST /api/external/form/{templateId}
    /// </summary>
    /// <param name="templateId">ID do template associado ao formulário.</param>
    /// <param name="formData">Objeto JSON contendo todos os campos preenchidos pelo cliente.</param>
    [HttpPost("{templateId}")]
    public async Task<IActionResult> PostFormSubmission(int templateId, [FromBody] object formData)
    {
        if (formData == null)
        {
            return BadRequest(new { Message = "Dados do formulário não podem ser vazios." });
        }

        // 1. Serializa o objeto JSON em uma string para armazenar no DB.
        var formDataJson = JsonSerializer.Serialize(formData);

        // 2. Tenta extrair campos principais (adapte conforme seu formulário)
        // Isso é crucial para que você possa buscar leads facilmente no DB
        var element = (JsonElement)formData;
        string leadName = element.TryGetProperty("NomeCompleto", out var nameProp) ? nameProp.GetString() : "N/D";
        string contactEmail = element.TryGetProperty("EmailContato", out var emailProp) ? emailProp.GetString() : "N/D";

        // 3. Chamada ao Core.Application para persistir
        var submission = await _formService.SubmitFormAsync(
            leadName,
            contactEmail,
            templateId,
            formDataJson
        );

        return Ok(new { message = "Formulário submetido com sucesso. ID: " + submission.Id });
    }
}