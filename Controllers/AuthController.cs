using JWTAuth.DTOs;
using JWTAuth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;

namespace JWTAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly GraphServiceClient _graphServiceClient;

        public AuthController(IUserService userService, GraphServiceClient graphServiceClient)
        {
            _userService = userService;
            _graphServiceClient = graphServiceClient;
        }

        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {

            var O365User = await _graphServiceClient.Me.Request().GetAsync();

            Console.WriteLine(O365User.Mail);

            AuthenticateRequestDTO model = new AuthenticateRequestDTO
            {
                Email = O365User.Mail,
                Password = "123456789"
            };

            var user = await _userService.Authenticate(model);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
    }
}
