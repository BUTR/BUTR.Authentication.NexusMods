using Microsoft.AspNetCore.Authentication;

namespace BUTR.Authentication.NexusMods.Authentication
{
    public sealed class ButrNexusModsAuthSchemeOptions : AuthenticationSchemeOptions
    {
        public string EncryptionKey { get; set; } = default!;
    }
}