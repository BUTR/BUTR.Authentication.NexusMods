using BUTR.Authentication.NexusMods.Authentication;
using BUTR.Authentication.NexusMods.Options;
using BUTR.Authentication.NexusMods.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System;

namespace BUTR.Authentication.NexusMods.Extensions
{
    public static class ButrNexusModsExtensions
    {
        public static AuthenticationBuilder AddNexusMods(this AuthenticationBuilder builder, Action<ButrNexusModsAuthSchemeOptions> configureOptions) => builder
            .AddScheme<ButrNexusModsAuthSchemeOptions, ButrNexusModsAuthHandler>(ButrNexusModsAuthSchemeConstants.AuthScheme, configureOptions);

        public static IServiceCollection AddNexusModsDefaultServices(this IServiceCollection services)
        {
            services.Configure<ButrNexusModsKeyValidatorOptions>(opt =>
            {
                opt.ApiEndpoint = "https://api.nexusmods.com/";
            });
            services.ConfigureOptions<RouteEncryptionKeyFromNexusModsAuthSchemeOptions>();

            services.AddHttpClient<INexusModsKeyValidator, NexusModsKeyValidator>().ConfigureHttpClient((sp, client) =>
            {
                var opt = sp.GetRequiredService<IOptions<ButrNexusModsKeyValidatorOptions>>().Value;
                client.BaseAddress = new Uri(opt.ApiEndpoint);
            });

            services.AddScoped<ITokenGenerator, DefaultTokenGenerator>();
            services.AddSingleton<ITokenBlacklistProvider, DefaultTokenBlacklistProvider>();

            return services;
        }
    }
}