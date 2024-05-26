using BUTR.Authentication.NexusMods.Authentication;
using BUTR.Authentication.NexusMods.Options;

using Microsoft.Extensions.Options;

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BUTR.Authentication.NexusMods.Services
{
    /// <summary>
    /// The default implementation of <see cref="INexusModsTokenValidator"/>. Uses <see cref="ButrNexusModsTokenValidatorOptions"/> for options.
    /// Needs special configuration. See the example below.
    /// <example>
    /// <code>
    /// <para/>services.AddHttpClient&lt;INexusModsTokenValidator, NexusModsTokenValidator&gt;().ConfigureHttpClient((sp, client) =>
    /// <para/>{
    /// <para/>    var opt = sp.GetRequiredService&lt;IOptions&lt;NexusModsTokenValidatorOptions&gt;&gt;().Value;
    /// <para/>    client.BaseAddress = new Uri(opt.UsersEndpoint);
    /// <para/>});
    /// </code>
    /// </example>
    /// </summary>
    public sealed class DefaultNexusModsTokenValidator : INexusModsTokenValidator
    {
        public sealed record NexusModsUserInfoResponse(
            [property: JsonPropertyName("sub")] string UserId,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("email")] string? Email,
            [property: JsonPropertyName("membership_roles")] string[] MembershipRoles);

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public DefaultNexusModsTokenValidator(HttpClient httpClient, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonSerializerOptions = jsonSerializerOptions.Value ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
        }

        public async Task<NexusModsUserInfo?> Validate(string accessToken, string refreshToken)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "oauth/userinfo");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return null;

                var responseType = await response.Content.ReadFromJsonAsync<NexusModsUserInfoResponse>(_jsonSerializerOptions);
                if (responseType is null)
                    return null;

                return new()
                {
                    UserId = uint.Parse(responseType.UserId),
                    Name = responseType.Name,
                    EMail = responseType.Email ?? "",
                    ProfileUrl = $"https://www.nexusmods.com/users/{responseType.UserId}",
                    IsSupporter = responseType.MembershipRoles.Contains("supporter"),
                    IsPremium = responseType.MembershipRoles.Contains("supporter"),
                    APIKey = null,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}