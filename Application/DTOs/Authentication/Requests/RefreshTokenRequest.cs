using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Authentication.Requests
{
    /// <summary>
    /// Request DTO for refreshing access token
    /// </summary>
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Access token is required")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
