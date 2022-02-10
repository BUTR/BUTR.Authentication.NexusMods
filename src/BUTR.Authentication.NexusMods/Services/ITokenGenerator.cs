using BUTR.Authentication.NexusMods.Authentication;
using BUTR.Authentication.NexusMods.Options;

using System.Threading.Tasks;

namespace BUTR.Authentication.NexusMods.Services
{
    /// <summary>
    /// We expose <see cref="TokenGeneratorOptions"/> for options. It's recommended to use it.
    /// </summary>
    public interface ITokenGenerator
    {
        Task<string> GenerateTokenAsync(ButrNexusModsUserInfo userInfo);
    }
}