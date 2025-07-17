using Domain.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{

    public interface IAuthService
    {
        Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterRequestDto request);
        Task<(bool Success, string Token, IEnumerable<string> Roles)> LoginAsync(LoginRequestDto request);
    }
}
