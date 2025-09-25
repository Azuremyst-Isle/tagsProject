using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace RfidApi.Authentication;

public class DummyAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    [Obsolete]
    public DummyAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    )
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new System.Security.Claims.ClaimsIdentity("DummyScheme");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "DummyScheme");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
