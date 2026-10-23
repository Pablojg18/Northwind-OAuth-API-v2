using APIWeb.Models;
using APIWeb.Models.Dtos;
using APIWeb.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace APIWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicantUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicantUser> userManager,
        TokenService tokenService,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicantUser
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Email.Split('@')[0]
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(error => new
            {
                error.Code,
                error.Description
            }));
        }

        return Ok(new { message = "Usuario creado correctamente. Ya puedes iniciar sesión." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Correo o contraseña incorrectos." });
        }

        return Ok(BuildResponse(user));
    }

    [HttpPost("google")]
    public async Task<IActionResult> Google([FromBody] GoogleLoginRequest request)
    {
        var clientId = _configuration["Authentication:Google:ClientId"];

        if (string.IsNullOrWhiteSpace(clientId))
        {
            return BadRequest(new { message = "Google OAuth no está configurado en el servidor." });
        }

        GoogleJsonWebSignature.Payload payload;

        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [clientId]
            });
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Token de Google inválido.");
            return Unauthorized(new { message = "El token de Google no es válido o expiró." });
        }

        if (!payload.EmailVerified)
        {
            return Unauthorized(new { message = "El correo de Google no está verificado." });
        }

        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user is null)
        {
            user = new ApplicantUser
            {
                UserName = payload.Email,
                Email = payload.Email,
                EmailConfirmed = true,
                Name = payload.Name,
                Picture = payload.Picture
            };

            var created = await _userManager.CreateAsync(user);

            if (!created.Succeeded)
            {
                return BadRequest(created.Errors.Select(error => new
                {
                    error.Code,
                    error.Description
                }));
            }
        }
        else if (user.Name != payload.Name || user.Picture != payload.Picture)
        {
            user.Name = payload.Name ?? user.Name;
            user.Picture = payload.Picture ?? user.Picture;
            await _userManager.UpdateAsync(user);
        }

        return Ok(BuildResponse(user));
    }

    [HttpGet("google-config")]
    public IActionResult GoogleConfig()
    {
        var clientId = _configuration["Authentication:Google:ClientId"];

        return Ok(new
        {
            configured = !string.IsNullOrWhiteSpace(clientId),
            clientId = clientId ?? string.Empty
        });
    }

    private AuthResponse BuildResponse(ApplicantUser user) => new()
    {
        Token = _tokenService.CreateToken(user),
        User = new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Name = user.Name,
            Picture = user.Picture
        }
    };
}
