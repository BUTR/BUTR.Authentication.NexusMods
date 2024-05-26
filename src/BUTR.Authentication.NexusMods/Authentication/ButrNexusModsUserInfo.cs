using System.Collections.Generic;

namespace BUTR.Authentication.NexusMods.Authentication
{
    public sealed record ButrNexusModsUserInfo
    {
        public required uint UserId { get; init; }
        public required string Name { get; init; }
        public required string EMail { get; init; }
        public required string ProfileUrl { get; init; }
        public required bool IsSupporter { get; init; }
        public required bool IsPremium { get; init; }
        public required string? APIKey { get; init; }
        public required string? AccessToken { get; init; }
        public required string? RefreshToken { get; init; }
        public required string Role { get; init; }
        public required Dictionary<string, string> Metadata { get; init; }
    };
}