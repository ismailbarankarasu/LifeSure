using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace LifeSure.Extensions;

public static class LocalizationServiceRegistration
{
    public static IServiceCollection AddSiteLocalization(
        this IServiceCollection services)
    {
        services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("tr-TR"),
                new CultureInfo("en-US")
            };

            options.DefaultRequestCulture =
                new RequestCulture("tr-TR");

            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            options.RequestCultureProviders.Clear();

            options.RequestCultureProviders.Add(
                new CookieRequestCultureProvider());

            options.ApplyCurrentCultureToResponseHeaders = true;
        });

        return services;
    }
}