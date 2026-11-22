using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application;
using Application.Common.Behaviors;
using Application.Common.Settings;
using Application.Features.Driver.DomainServices;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = typeof(ApplicationAssemblyMarker).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Both behaviors existed but were never registered, so no
            // validator ever ran and requests were never logged. Validation
            // runs first so an invalid command never reaches a handler.
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        // Cross-aggregate orchestration service — resolves its repositories directly from DI
        services.AddScoped<IDriverShiftService, DriverShiftService>();

        // Settings owned by the Application layer (bound once, injected via IOptions)
        services.Configure<ApiSettings>(configuration.GetSection(ApiSettings.SectionName));

        return services;
    }
}
