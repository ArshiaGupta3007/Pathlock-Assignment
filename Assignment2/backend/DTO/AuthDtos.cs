using System.ComponentModel.DataAnnotations;

namespace ProjectManagerApi.DTOs
{
    public class RegisterDto
    {
        [Required, MinLength(3), MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;
    }

    public class LoginDto
    {
        [Required] public string Username { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
    }
}
