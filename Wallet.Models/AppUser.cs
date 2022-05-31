using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Wallet.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}