using System.ComponentModel;

namespace JWTAuth.DTOs
{
    public class AuthenticateRequestDTO
    {
        [DefaultValue("System")]
        public required string Email { get; set; }

        [DefaultValue("System")]
        public required string Password { get; set; }
    }
}
