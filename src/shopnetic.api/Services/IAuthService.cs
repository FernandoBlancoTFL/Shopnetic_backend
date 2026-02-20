using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shopnetic.API.Dto;
using Shopnetic.API.Models;

namespace Shopnetic.API.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserRequestDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
        Task<UserDto?> GetCurrentUserAsync(int userIdClaim);

    }
}