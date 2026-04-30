using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Produkty24_API
{
    public class StaticTokenAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultSchemeName = "StaticTokenAuthenticationScheme";
        public string TokenHeaderName { get; set; } = "X-Auth-Token";
    }

    public class StaticTokenAuthHandler : AuthenticationHandler<StaticTokenAuthOptions>
    {
        public StaticTokenAuthHandler(IOptionsMonitor<StaticTokenAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var tokenValue = "DepecheModePersonalJesus"; // I know this is not the best solution

            if (!Request.Headers.ContainsKey(Options.TokenHeaderName))
                return Task.FromResult(AuthenticateResult.Fail($"Missing Header For Token: {Options.TokenHeaderName}"));

            if (Request.Headers[Options.TokenHeaderName] != tokenValue)
                return Task.FromResult(AuthenticateResult.Fail("Token Values Mismatch"));

            var token = Request.Headers[Options.TokenHeaderName];

            var username = "Username-From-Somewhere-By-Token";
            var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Name, username),
        };
            var id = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(id);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
