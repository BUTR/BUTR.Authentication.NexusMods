using BUTR.Authentication.NexusMods.Authentication;
using BUTR.Authentication.NexusMods.Options;

using Microsoft.Extensions.Options;

using System;

namespace BUTR.Authentication.NexusMods.Extensions
{
    /// <summary>
    /// Our little hack to inject EncryptionKey to <see cref="TokenGeneratorOptions"/> from <see cref="ButrNexusModsAuthSchemeOptions"/>.
    /// </summary>
    public sealed class RouteEncryptionKeyFromNexusModsAuthSchemeOptions : IConfigureOptions<TokenGeneratorOptions>
    {
        private readonly IOptionsMonitor<ButrNexusModsAuthSchemeOptions> _optionsMonitor;

        public RouteEncryptionKeyFromNexusModsAuthSchemeOptions(IOptionsMonitor<ButrNexusModsAuthSchemeOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        public void Configure(TokenGeneratorOptions options)
        {
            options.EncryptionKey = _optionsMonitor.Get(ButrNexusModsAuthSchemeConstants.AuthScheme).EncryptionKey;
        }
    }
}