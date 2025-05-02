using Entity.DTOs;

namespace Business.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> AuthenticateAsync(AuthRequestDTO request);
        Task<AccessTokenDTO> GenerateTokenAsync(string username);
    }
}
