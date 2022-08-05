using System.Collections.Generic;

namespace BUTR.Authentication.NexusMods.Authentication
{
    public sealed record ButrNexusModsUserInfo
    {
        public uint UserId { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string EMail { get; init; } = default!;
        public string ProfileUrl { get; init; } = default!;
        public bool IsSupporter { get; init; } = default!;
        public bool IsPremium { get; init; } = default!;
        public string APIKey { get; init; } = default!;
        public string Role { get; init; } = default!;
        public Dictionary<string, string> Metadata { get; init; } = default!;
    };
}