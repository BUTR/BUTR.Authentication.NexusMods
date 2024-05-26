namespace BUTR.Authentication.NexusMods.Options;

public sealed record ButrNexusModsTokenValidatorOptions
{
    public string UsersEndpoint { get; set; } = default!;
}