using System.Collections.Generic;
using System.Security.Claims;

namespace BUTR.Authentication.NexusMods.Authentication
{
    public sealed class ButrNexusModsIdentity : ClaimsIdentity
    {
        public ButrNexusModsIdentity(IEnumerable<Claim>? claims) : base(claims, nameof(ButrNexusModsAuthHandler), ButrNexusModsClaimTypes.Name, ButrNexusModsClaimTypes.Role) { }
    }
}