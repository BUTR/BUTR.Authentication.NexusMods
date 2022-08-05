using BUTR.Authentication.NexusMods.Authentication;
using BUTR.Authentication.NexusMods.Options;

using System;
using System.Threading.Tasks;

namespace BUTR.Authentication.NexusMods.Services
{
    public sealed record GeneratedToken(string Token, Guid TokenUid, DateTime CreationTime);

    /// <summary>
    /// We expose <see cref="TokenGeneratorOptions"/> for options. It's recommended to use it.
    /// </summary>
    public interface ITokenGenerator
    {
        Task<GeneratedToken> GenerateTokenAsync(ButrNexusModsUserInfo userInfo);
    }
}