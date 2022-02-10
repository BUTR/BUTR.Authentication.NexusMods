using System;

namespace BUTR.Authentication.NexusMods.Authentication
{
    public sealed record ButrNexusModsTokenData
    {
        public ulong UserId { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string EMail { get; init; } = default!;
        public string ProfileUrl { get; init; } = default!;
        public bool IsSupporter { get; init; } = default!;
        public bool IsPremium { get; init; } = default!;
        public string APIKey { get; init; } = default!;
        public string Role { get; init; } = default!;

        public Guid TokenUid { get; init; } = default!;
        public DateTime CreationTime { get; init; } = default!;
    }
}