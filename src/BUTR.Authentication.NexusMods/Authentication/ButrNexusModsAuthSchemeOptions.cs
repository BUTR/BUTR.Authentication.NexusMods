using Microsoft.AspNetCore.Authentication;

using System.Diagnostics.CodeAnalysis;

namespace BUTR.Authentication.NexusMods.Authentication
{
    public sealed class ButrNexusModsAuthSchemeOptions : AuthenticationSchemeOptions
    {
        public required string EncryptionKey { get; set; } = default!;

        [SetsRequiredMembers]
        public ButrNexusModsAuthSchemeOptions() { }
    }
}