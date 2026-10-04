using GDB.Api.Application.Dtos.Request;

namespace GDB.Api.Application.Services.Contracts
{
    public interface IAuthService
    {
        string? Login(LoginRequest request);
    }
}