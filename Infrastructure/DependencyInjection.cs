using Domain.Interfaces;
using Domain.Interfaces.Messaging;
using Domain.Interfaces.Services;
using Infrastructure.Messaging;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. تسجيل الـ Interceptors
            services.AddScoped<EntitySaveChangesInterceptor>();
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            // 2. تسجيل الـ DbContext مع الـ Interceptor
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<EntitySaveChangesInterceptor>();

                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                       .AddInterceptors(interceptor);
            });

            // 3. تسجيل الـ Repositories والـ Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 4. تسجيل الـ Infrastructure Services
            services.AddHttpContextAccessor(); // مهم جداً عشان الـ CurrentUserService يشتغل
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            // 5. تسجيل الـ Messaging (RabbitMQ & MassTransit)
            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"]);
                        h.Password(configuration["RabbitMQ:Password"]);
                    });
                });
            });

            services.AddScoped<IMessageBus, MessageBus>();

            return services;
        }
    }
}