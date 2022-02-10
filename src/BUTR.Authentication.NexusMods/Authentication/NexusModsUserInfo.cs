namespace BUTR.Authentication.NexusMods.Authentication
{
    public sealed record NexusModsUserInfo
    {
        public uint UserId { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string EMail { get; init; } = default!;
        public string ProfileUrl { get; init; } = default!;
        public bool IsSupporter { get; init; } = default!;
        public bool IsPremium { get; init; } = default!;
        public string APIKey { get; init; } = default!;
    };
}