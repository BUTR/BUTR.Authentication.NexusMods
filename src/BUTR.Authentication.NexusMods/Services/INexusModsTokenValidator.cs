using BUTR.Authentication.NexusMods.Authentication;

using System.Threading.Tasks;

namespace BUTR.Authentication.NexusMods.Services;

/// <summary>
/// Checks whether the Bearer Token provided is valid.
/// </summary>
public interface INexusModsTokenValidator
{
    Task<NexusModsUserInfo?> Validate(string accessToken, string? refreshToken);
}