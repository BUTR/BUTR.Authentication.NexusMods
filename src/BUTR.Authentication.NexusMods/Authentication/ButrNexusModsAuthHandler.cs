using BUTR.Authentication.NexusMods.Services;
using BUTR.Authentication.NexusMods.Utils;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace BUTR.Authentication.NexusMods.Authentication
{
    public sealed class ButrNexusModsAuthHandler : AuthenticationHandler<ButrNexusModsAuthSchemeOptions>
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ITokenBlacklistProvider _tokenBlacklistProvider;
        private readonly INexusModsApiKeyValidator _nexusModsApiKeyValidator;
        private readonly INexusModsTokenValidator _nexusModsTokenValidator;
        private readonly IHostEnvironment _environment;

        public ButrNexusModsAuthHandler(
            IOptionsMonitor<ButrNexusModsAuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,

            IOptions<JsonSerializerOptions> jsonSerializerOptions,
            ITokenBlacklistProvider tokenBlacklistProvider,
            INexusModsApiKeyValidator nexusModsApiKeyValidator,
            INexusModsTokenValidator nexusModsTokenValidator,
            IHostEnvironment environment) : base(options, logger, encoder, clock)
        {
            _jsonSerializerOptions = jsonSerializerOptions.Value ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
            _tokenBlacklistProvider = tokenBlacklistProvider ?? throw new ArgumentNullException(nameof(tokenBlacklistProvider));
            _nexusModsApiKeyValidator = nexusModsApiKeyValidator ?? throw new ArgumentNullException(nameof(nexusModsApiKeyValidator));
            _nexusModsTokenValidator = nexusModsTokenValidator ?? throw new ArgumentNullException(nameof(nexusModsTokenValidator));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = string.Empty;

            if (string.IsNullOrEmpty(token) && Request.Query.TryGetValue("access_token", out var strings))
            {
                token = strings.ToString();
            }
            if (string.IsNullOrEmpty(token) && Request.Headers.TryGetValue(HeaderNames.Authorization, out strings))
            {
                var marker = $"{Scheme.Name} ";
                var value = strings.ToString();
                if (!value.StartsWith(marker))
                {
                    return AuthenticateResult.Fail("Wrong Sheme!");
                }
                token = value.Substring(marker.Length);
            }

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("Token Not Found!");
            }

            var decryptedToken = CryptographyManager.Decrypt(token, Options.EncryptionKey) ?? string.Empty;

            ButrNexusModsTokenData? model;
            try
            {
                model = JsonSerializer.Deserialize<ButrNexusModsTokenData?>(decryptedToken, _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                if (_environment.IsDevelopment())
                {
                    return AuthenticateResult.Fail(ex);
                }

                return AuthenticateResult.Fail("Failed to parse token!");
            }

            if (model is null)
            {
                return AuthenticateResult.Fail("Token is empty!");
            }

            if (DateTime.UtcNow - model.CreationTime > TimeSpan.FromDays(1))
            {
                return AuthenticateResult.Fail("Token is expired!");
            }

            if (_tokenBlacklistProvider.IsBlacklisted(model.TokenUid))
            {
                return AuthenticateResult.Fail("Token is blacklisted!");
            }
            
            if (!string.IsNullOrEmpty(model.APIKey))
            {
                if (await _nexusModsApiKeyValidator.Validate(model.APIKey) is not { } userInfo)
                {
                    return AuthenticateResult.Fail("Invalid NexusMods API Key!");
                }
                
                if (!Compare(model, userInfo))
                {
                    return AuthenticateResult.Fail("NexusMods data has changed!");
                }
            }

            if (!string.IsNullOrEmpty(model.AccessToken))
            {
                if (await _nexusModsTokenValidator.Validate(model.AccessToken, model.RefreshToken) is not { } userInfo)
                {
                    return AuthenticateResult.Fail("Invalid NexusMods Access Token!");
                }
                
                if (!Compare(model, userInfo))
                {
                    return AuthenticateResult.Fail("NexusMods data has changed!");
                }
            }

            var claims = new[] {
                new Claim(ButrNexusModsClaimTypes.UserId, model.UserId.ToString()),
                new Claim(ButrNexusModsClaimTypes.Name, model.Name),
                new Claim(ButrNexusModsClaimTypes.EMail, model.EMail),
                new Claim(ButrNexusModsClaimTypes.ProfileUrl, model.ProfileUrl),
                new Claim(ButrNexusModsClaimTypes.IsSupporter, model.IsSupporter.ToString()),
                new Claim(ButrNexusModsClaimTypes.IsPremium, model.IsPremium.ToString()),
                new Claim(ButrNexusModsClaimTypes.APIKey, model.APIKey ?? ""),
                new Claim(ButrNexusModsClaimTypes.AccessToken, model.AccessToken ?? ""),
                new Claim(ButrNexusModsClaimTypes.RefreshToken, model.RefreshToken ?? ""),
                new Claim(ButrNexusModsClaimTypes.Role, model.Role),
                new Claim(ButrNexusModsClaimTypes.Metadata, JsonSerializer.Serialize(model.Metadata, _jsonSerializerOptions)),

                new Claim(ButrNexusModsClaimTypes.TokenUid, model.TokenUid.ToString()),
                new Claim(ButrNexusModsClaimTypes.CreationTime, model.CreationTime.ToString("o"))
            };
            return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ButrNexusModsIdentity(claims)), Scheme.Name));
        }

        private static bool Compare(ButrNexusModsTokenData current, NexusModsUserInfo nexusModsData)
        {
            if (current.UserId != nexusModsData.UserId)
                return false;
            if (current.Name != nexusModsData.Name)
                return false;
            if (current.EMail != nexusModsData.EMail)
                return false;
            if (current.ProfileUrl != nexusModsData.ProfileUrl)
                return false;
            if (current.IsSupporter != nexusModsData.IsSupporter)
                return false;
            if (current.IsPremium != nexusModsData.IsPremium)
                return false;
            if (current.APIKey != nexusModsData.APIKey)
                return false;
            if (current.AccessToken != nexusModsData.AccessToken)
                return false;
            if (current.RefreshToken != nexusModsData.RefreshToken)
                return false;
            return true;
        }
    }
}