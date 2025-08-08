using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenVmsTextEditor.Domain;
using OpenVmsTextEditor.Domain.Interfaces;
using File = OpenVmsTextEditor.Domain.Models.File;

namespace OpenVmsTextEditor.Infrastructure;

public class VmsIo : IOperatingSystemIo
{
    private readonly ILogger<VmsIo> _logger;
    private readonly VmsEditorSettings _vmsEditorSettings;
    private readonly IOptions<JwtIssueOptions> _jwtOpt;
    private readonly IHttpContextAccessor _http;
    private readonly RsaSecurityKey _signingKey;

    // this signature constructor must not change
    public VmsIo(ILoggerFactory loggerFactory, 
        IOptions<VmsEditorSettings> vmsEditorSettings,
        IOptions<JwtIssueOptions> jwtOpt,
        IHttpContextAccessor httpContextAccessor, 
        RsaSecurityKey securityKey)
    {
        _logger = loggerFactory.CreateLogger<VmsIo>();
        _vmsEditorSettings = vmsEditorSettings.Value;
        
        _jwtOpt = jwtOpt;
        _http = httpContextAccessor;
        _signingKey = securityKey;
    }

    public async Task<IList<string>> GetDisks()
    {
        _logger.LogDebug("GetDisks()");

        using var client = new HttpClient();
        try
        {
            var vmsExplorerApiUrl = _vmsEditorSettings.VmsExplorerApiUrl;

            _logger.LogDebug("vmsExplorerApiUrl = {uri}", vmsExplorerApiUrl);

            client.DefaultRequestHeaders.Add("accept", "application/json");
            var requestUri = $"{vmsExplorerApiUrl}/api/disk";
            _logger.LogDebug("requestUri = {uri}", requestUri);

            var response = await client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<string>>(result) ?? new List<string>();
            }

            throw new Exception($"{response.StatusCode} - {response.ReasonPhrase}");
        }
        catch (Exception e)
        {
            _logger.LogError(e,e.Message);
            throw;
        }
    }


    public async Task<IList<File>> GetDirectoryFiles(string? include, string? exclude, bool showHistory, string fullFolderName)
    {
        _logger.LogDebug("GetDirectoryFiles({0})", fullFolderName);

        using var client = new HttpClient();
        try
        {
            var vmsExplorerApiUrl = _vmsEditorSettings.VmsExplorerApiUrl;

            _logger.LogDebug("vmsExplorerApiUrl = {0}", vmsExplorerApiUrl);

            
            
            // ---- NEW: mint short-lived JWT from current user and attach as Bearer ----
            var user = _http.HttpContext?.User;
            if (!(user?.Identity?.IsAuthenticated ?? false))
                throw new UnauthorizedAccessException("User must be authenticated.");

            var token = IssueUserJwt(user!, _signingKey, _jwtOpt.Value);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // -------------------------------------------------------------------------
            
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("accept", "application/json");
            var requestUri = $"{vmsExplorerApiUrl}/api/directory/{fullFolderName}?showHistory={showHistory}";
            if (!string.IsNullOrWhiteSpace(include)) requestUri += $"&include={include}";
            if (!string.IsNullOrWhiteSpace(exclude)) requestUri += $"&exclude={exclude}";
            _logger.LogDebug("requestUri = {0}", requestUri);

            var response = await client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<File>>(result) ?? new List<File>();
            }

            throw new Exception($"{response.StatusCode} - {response.ReasonPhrase}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<string> GetFile(string fullFileName)
    {
        _logger.LogDebug("GetFile({0})", fullFileName);

        using var client = new HttpClient();
        try
        {
            var vmsExplorerApiUrl = _vmsEditorSettings.VmsExplorerApiUrl;

            _logger.LogDebug("vmsExplorerApiUrl = {0}", vmsExplorerApiUrl);

            // todo, yikes, I may need to know if it is vms or windows to choose the charset

            client.DefaultRequestHeaders.Add("accept", "text/plain");
            var requestUri = $"{vmsExplorerApiUrl}/api/file/{fullFileName}";
            _logger.LogDebug("requestUri = {0}", requestUri);

            var response = await client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            throw new Exception($"{response.StatusCode} - {response.ReasonPhrase}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<string> SaveFile(string fullFileName, string fileData)
    {
        _logger.LogDebug("SaveFile({0})", fullFileName);
        using var client = new HttpClient();
        try
        {
            var vmsExplorerApiUrl = _vmsEditorSettings.VmsExplorerApiUrl;

            _logger.LogDebug("vmsExplorerApiUrl = {url}", vmsExplorerApiUrl);

            // todo, yikes, I may need to know if it is vms or windows to choose the charset

            client.DefaultRequestHeaders.Add("accept", "text/plain");
            var requestUri = $"{vmsExplorerApiUrl}/api/file/{fullFileName}";
            _logger.LogDebug("requestUri = {0}", requestUri);


            var iso = Encoding.GetEncoding("ISO-8859-1");
            var dflt = Encoding.Default;
            var utfBytes = dflt.GetBytes(fileData);
            var isoBytes = Encoding.Convert(dflt, iso, utfBytes);
            //string msg = iso.GetString(isoBytes);


            var response = await client.PostAsync(requestUri, new ByteArrayContent(isoBytes));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            throw new Exception($"{response.StatusCode} - {response.ReasonPhrase}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
    
    
    
    private static string IssueUserJwt(ClaimsPrincipal principal, RsaSecurityKey key, JwtIssueOptions opts)
    {
        var now = DateTimeOffset.UtcNow;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty),
            new(ClaimTypes.Name, principal.Identity?.Name ?? string.Empty),
            // Optional: include roles so the Java API can authorize by role
            // .. principal.FindAll(ClaimTypes.Role).Select(r => new Claim("roles", r.Value))
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
        var jwt = new JwtSecurityToken(
            issuer: opts.Issuer,
            audience: opts.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: now.AddMinutes(5).UtcDateTime, // short-lived hop token
            signingCredentials: creds);

        if (!string.IsNullOrEmpty(opts.KeyId))
            jwt.Header["kid"] = opts.KeyId;

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    
    
}
// matches your Program.cs binding
public record JwtIssueOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string KeyId { get; init; } = "kid-1";
}

