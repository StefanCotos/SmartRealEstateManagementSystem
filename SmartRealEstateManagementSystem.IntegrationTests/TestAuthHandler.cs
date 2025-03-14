using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        [Obsolete("This constructor is obsolete and will be removed in future versions.")]
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                    new Claim(ClaimTypes.Name, "testuser"),
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    // Add more claims if your controllers require them
                    // Example:
                    // new Claim(ClaimTypes.Role, "Admin")
                };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }
    }
}
