using BUTR.Authentication.NexusMods.Authentication;

using System.Threading.Tasks;

namespace BUTR.Authentication.NexusMods.Services
{
    /// <summary>
    /// Checks whether the API Key provided is valid.
    /// </summary>
    public interface INexusModsKeyValidator
    {
        Task<NexusModsUserInfo?> ValidateAPIKey(string apiKey);
    }
}