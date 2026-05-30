using Application.Features.Users.Services;
using Domain.Claims;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Inventory;
using Domain.Invoices;
using Domain.Users;
using Domain.Vehicles;
using Domain.Warehouse;
using EducationalPlatform.Infrastructure.Services.Token;
using Infrastructure.Identity;
using Infrastructure.Presistence.Data;
using Infrastructure.Presistence.Interceptors;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Shared;
using Infrastructure.Repositories.Vehicle.Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ✅ 1. Register Interceptor
            services.AddScoped<DomainEventInterceptor>();

            // ✅ 2. Register DbContext
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<DomainEventInterceptor>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                options.UseSqlServer(connectionString);
                options.AddInterceptors(interceptor);
            });

            // ✅ 3. Register UnitOfWork 
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ✅ 4. Register Repositories 
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IClaimRepository,ClaimRepository>();

            // services.AddScoped<IDriverRepository, DriverRepository>();
            // services.AddScoped<ITripRepository, TripRepository>();
            // 7. Register Services
            services.AddHttpContextAccessor();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITotpService, TotpService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddDataProtection();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}