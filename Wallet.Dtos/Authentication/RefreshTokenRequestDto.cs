using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Dtos.Authentication
{
    public class RefreshTokenRequestDto
    {
        public string? UserId { get; set; }
        public string? RefreshToken { get; set; }
    }
}
