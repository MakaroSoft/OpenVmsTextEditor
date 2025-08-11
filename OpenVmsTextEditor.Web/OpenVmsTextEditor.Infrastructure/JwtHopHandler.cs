using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace OpenVmsTextEditor.Infrastructure;

public sealed class JwtHopHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _http;
    private readonly RsaSecurityKey _key;
    private readonly IOptions<JwtIssueOptions> _jwt;

    public JwtHopHandler(IHttpContextAccessor http, RsaSecurityKey key, IOptions<JwtIssueOptions> jwt)
    {
        _http = http; _key = key; _jwt = jwt;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var user = _http.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var token = IssueUserJwt(user, _key, _jwt.Value);  // your helper
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, ct);
    }

    private static string IssueUserJwt(ClaimsPrincipal principal, RsaSecurityKey key, JwtIssueOptions opts)
    {
        var now = DateTimeOffset.UtcNow;
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? ""),
            new Claim(ClaimTypes.Name, principal.Identity?.Name ?? "")
        };

        // add the roles
        claims.AddRange(
            principal.FindAll(ClaimTypes.Role)
                .Select(c => new Claim(ClaimTypes.Role, c.Value))
        );
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
        var jwt = new JwtSecurityToken(opts.Issuer, opts.Audience, claims, now.UtcDateTime, now.AddMinutes(5).UtcDateTime, creds);
        if (!string.IsNullOrEmpty(opts.KeyId)) jwt.Header["kid"] = opts.KeyId;
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

}
