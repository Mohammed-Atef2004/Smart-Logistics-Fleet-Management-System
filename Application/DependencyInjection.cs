using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Application;
using Application.Common.Settings;
using Application.Features.Driver.Commands.AssignShift;
using Application.Features.Driver.DomainServices;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

        // Cross-aggregate orchestration service (repos are exposed via IUnitOfWork, not registered individually)
        services.AddScoped<IDriverShiftService>(sp =>
        {
            var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
            return new DriverShiftService(unitOfWork.Drivers, unitOfWork.Shifts);
        });

        // Settings owned by the Application layer (bound once, injected via IOptions)
        services.Configure<ApiSettings>(configuration.GetSection(ApiSettings.SectionName));        return services;
    }
}
