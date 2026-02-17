using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Authentication.Requests
{
    public class GoogleLoginRequest
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
