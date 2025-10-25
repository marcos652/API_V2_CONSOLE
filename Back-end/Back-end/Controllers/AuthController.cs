// File: Web.Api/Controllers/AuthController.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Ports.Input;
using Web.Api.DTOs; // Assume que existem RegisterDTO e LoginDTO

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Cadastra um novo usuário no sistema (Cadastro.html).
    /// Endpoint: POST /api/auth/register
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        // Validação de entrada...

        var user = await _authService.RegisterAsync(dto.Username, dto.Email, dto.Password);

        return Ok(new { Message = "Usuário cadastrado com sucesso!", UserId = user.Id });
    }

    /// <summary>
    /// Realiza o login e retorna um Token JWT (Login.html).
    /// Endpoint: POST /api/auth/login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        try
        {
            var token = await _authService.LoginAsync(dto.Username, dto.Password);

            // Retorna o token para o frontend armazenar e usar em chamadas futuras.
            return Ok(new { Token = token, Message = "Login efetuado com sucesso." });
        }
        catch (System.UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
    }
}