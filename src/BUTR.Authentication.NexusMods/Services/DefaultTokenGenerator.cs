using BUTR.Authentication.NexusMods.Authentication;
using BUTR.Authentication.NexusMods.Options;
using BUTR.Authentication.NexusMods.Utils;

using Microsoft.Extensions.Options;

using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace BUTR.Authentication.NexusMods.Services
{
    /// <summary>
    /// The default implementation of <see cref="ITokenGenerator"/>. Uses <see cref="TokenGeneratorOptions"/> for options.
    /// </summary>
    public sealed class DefaultTokenGenerator : ITokenGenerator
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly TokenGeneratorOptions _options;

        public DefaultTokenGenerator(IOptions<JsonSerializerOptions> jsonSerializerOptions, IOptions<TokenGeneratorOptions> options)
        {
            _jsonSerializerOptions = jsonSerializerOptions.Value ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<GeneratedToken> GenerateTokenAsync(ButrNexusModsUserInfo userInfo)
        {
            var model = new ButrNexusModsTokenData
            {
                UserId = userInfo.UserId,
                Name = userInfo.Name,
                EMail = userInfo.EMail,
                ProfileUrl = userInfo.ProfileUrl,
                IsSupporter = userInfo.IsSupporter,
                IsPremium = userInfo.IsPremium,
                APIKey = userInfo.APIKey,
                Role = userInfo.Role,
                Metadata = userInfo.Metadata,

                TokenUid = Guid.NewGuid(),
                CreationTime = DateTime.UtcNow,
            };
            var rawToken = JsonSerializer.Serialize(model, _jsonSerializerOptions);
            return Task.FromResult(new GeneratedToken(CryptographyManager.Encrypt(rawToken, _options.EncryptionKey), model.TokenUid, model.CreationTime));
        }
    }
}