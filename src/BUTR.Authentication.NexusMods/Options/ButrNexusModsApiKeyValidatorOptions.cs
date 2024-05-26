namespace BUTR.Authentication.NexusMods.Options
{
    public sealed record ButrNexusModsApiKeyValidatorOptions
    {
        public string ApiEndpoint { get; set; } = default!;
    }
}