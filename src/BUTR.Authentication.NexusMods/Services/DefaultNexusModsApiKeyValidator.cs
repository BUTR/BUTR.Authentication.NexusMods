using BUTR.Authentication.NexusMods.Authentication;
using BUTR.Authentication.NexusMods.Options;

using Microsoft.Extensions.Options;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BUTR.Authentication.NexusMods.Services
{
    /// <summary>
    /// The default implementation of <see cref="INexusModsApiKeyValidator"/>. Uses <see cref="ButrNexusModsApiKeyValidatorOptions"/> for options.
    /// Needs special configuration. See the example below.
    /// <example>
    /// <code>
    /// <para/>services.AddHttpClient&lt;INexusModsApiKeyValidator, NexusModsApiKeyValidator&gt;().ConfigureHttpClient((sp, client) =>
    /// <para/>{
    /// <para/>    var opt = sp.GetRequiredService&lt;IOptions&lt;NexusModsApiKeyValidatorOptions&gt;&gt;().Value;
    /// <para/>    client.BaseAddress = new Uri(opt.ApiEndpoint);
    /// <para/>});
    /// </code>
    /// </example>
    /// </summary>
    public sealed class DefaultNexusModsApiKeyValidator : INexusModsApiKeyValidator
    {
        private sealed record NexusModsValidateResponse
        {
            [JsonPropertyName("user_id")]
            public uint UserId { get; set; } = default!;

            [JsonPropertyName("key")]
            public string Key { get; set; } = default!;

            [JsonPropertyName("name")]
            public string Name { get; set; } = default!;

            //[JsonPropertyName("is_premium?")]
            //public bool IsPremium0 { get; set; } = default!;
            //
            //[JsonPropertyName("is_supporter?")]
            //public bool IsSupporter0 { get; set; } = default!;

            [JsonPropertyName("email")]
            public string? Email { get; set; } = default!;

            [JsonPropertyName("profile_url")]
            public string? ProfileUrl { get; set; } = default!;

            [JsonPropertyName("is_supporter")]
            public bool IsSupporter { get; set; } = default!;

            [JsonPropertyName("is_premium")]
            public bool IsPremium { get; set; } = default!;
        }

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public DefaultNexusModsApiKeyValidator(HttpClient httpClient, IOptions<JsonSerializerOptions> jsonSerializerOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonSerializerOptions = jsonSerializerOptions.Value ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
        }

        public async Task<NexusModsUserInfo?> Validate(string apiKey)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "v1/users/validate.json");
                request.Headers.Add("apikey", apiKey);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    return null;
                var responseType = await response.Content.ReadFromJsonAsync<NexusModsValidateResponse>(_jsonSerializerOptions);
                if (responseType is null || responseType.Key != apiKey)
                    return null;

                return new()
                {
                    UserId = responseType.UserId,
                    Name = responseType.Name,
                    EMail = responseType.Email,
                    ProfileUrl = responseType.ProfileUrl,
                    IsSupporter = responseType.IsSupporter,
                    IsPremium = responseType.IsPremium,
                    APIKey = responseType.Key,
                    AccessToken = null,
                    RefreshToken = null,
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}