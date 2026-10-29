using VKart.ModelsLayer.DTOs;
using VKart.ServicesLayer.Common;

namespace VKart.ServicesLayer.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<ServiceResult<AuthResponseDto>> GetCurrentUserAsync(string userId);
}