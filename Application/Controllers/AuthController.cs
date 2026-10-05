using GDB.Api.Application.Dtos.Request;
using GDB.Api.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GDB.Api.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var token = _authService.Login(request);

            if (token == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }

            return Ok(new
            {
                token = token
            });
        }
    }
}