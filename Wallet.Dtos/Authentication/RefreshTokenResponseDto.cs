using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Dtos.Authentication
{
    public class RefreshTokenToResponseDto
    {
        public string? NewJwtAccessToken { get; set; }
        public string? NewRefreshToken { get; set; }
    }
}
