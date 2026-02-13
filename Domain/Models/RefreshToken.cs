using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models
{
    [Owned]
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }  // FK  ApplicationUser
        public DateTime Expiration { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Helper Property
        public bool IsActive => !IsRevoked && DateTime.UtcNow <= Expiration;

        // Navigation Property
        public ApplicationUser User { get; set; } = null!;
    }
}
