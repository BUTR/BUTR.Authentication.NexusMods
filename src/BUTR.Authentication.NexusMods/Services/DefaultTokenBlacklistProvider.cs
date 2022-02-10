using System;

namespace BUTR.Authentication.NexusMods.Services
{
    /// <summary>
    /// The default implementation of <see cref="ITokenBlacklistProvider"/>. Just returns <see langword="false" />.
    /// </summary>
    public sealed class DefaultTokenBlacklistProvider : ITokenBlacklistProvider
    {
        public bool IsBlacklisted(Guid uid) => false;
    }
}