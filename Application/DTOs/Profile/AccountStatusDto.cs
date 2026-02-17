using System;

namespace Application.DTOs.Profile
{
    public class AccountStatusDto
    {
        public string Email { get; set; } = string.Empty;
        public bool IsEmailConfirmed { get; set; }
        public bool HasPassword { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}
