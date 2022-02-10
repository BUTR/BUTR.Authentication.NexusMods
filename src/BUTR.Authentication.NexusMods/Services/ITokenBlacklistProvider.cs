using System;

namespace BUTR.Authentication.NexusMods.Services
{
    /// <summary>
    /// Gives the ability to blacklist generated tokens based on the TokenId.
    /// </summary>
    public interface ITokenBlacklistProvider
    {
        bool IsBlacklisted(Guid uid);
    }
}