using Domain.Drivers;
using Domain.Inventory;
using Domain.Shifts;
using Domain.Shipments;
using Application.Common.Interfaces;
using Application.Features.Users.Services;
using Domain.Claims;
using Domain.Invoices;
using Domain.Payments;
using Domain.Users;
using Domain.Vehicles;
using Domain.Warehouse;
using Infrastructure.Identity;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Shared;
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
            services.AddScoped<DomainEventInterceptor>();

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<DomainEventInterceptor>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                options.UseSqlServer(connectionString);
                options.AddInterceptors(interceptor);
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Settings used by infrastructure services (bound once, injected via IOptions)
            services.Configure<Application.Common.Settings.EmailSettings>(
                configuration.GetSection(Application.Common.Settings.EmailSettings.SectionName));

            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IClaimRepository,ClaimRepository>();
            services.AddScoped<IPaymentRepository,PaymentRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<IShiftRepository, ShiftRepository>();
            services.AddScoped<IShipmentRepository, ShipmentRepository>();
            // IVehicleRepository already implements IVehicleUniquenessChecker,
            // but the domain service is injected on its own into Vehicle.Create flows.
            services.AddScoped<IVehicleUniquenessChecker, VehicleUniquenessChecker>();
            services.AddScoped<IDriverUniquenessChecker, DriverUniquenessChecker>();
            services.AddScoped<IPaymentGateway, MockPaymentGateway>();

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