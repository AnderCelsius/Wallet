using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Dtos.Authentication;
using Wallet.Utils;

namespace Wallet.Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Response<string>> Register(RegisterUserDto userDto);
        Task<Response<LoginResponseDto>> Login(LoginRequestDto loginDto);
        Task<Response<string>> ConfirmEmail(string email, string token);
        Task<Response<string>> ForgotPassword(string email);
        Task<Response<bool>> ChangePassword(string id, ChangePasswordDto changePasswordDto);
        Task<Response<string>> ResetPassword(ResetPasswordDto model);
        Task<Response<string>> UpdatePassword(UpdatePasswordDto model);
        Task<Response<RefreshTokenToResponseDto>> RefreshToken(RefreshTokenRequestDto refreshTokenRequestDto);
        Task<bool> ValidateUserRole(string userId, string[] roles);
    }
}
