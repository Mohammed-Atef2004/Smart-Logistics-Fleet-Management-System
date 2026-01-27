using Domain.Billing.Interfaces.Services;
using Domain.Interfaces.Repositories;
using Domain.Users.Interfaces.Services;
using Infrastructure.Messaging.OutBox;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Repositories.Shared;
using Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore; // مهم جداً
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. تسجيل الـ Interceptors
            services.AddScoped<EntitySaveChangesInterceptor>();

            // 2. تسجيل الـ DbContext (تعديل لضمان عدم حدوث NullReference)
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                // استخدام GetService بدل GetRequiredService عشان ميضربش لو الـ Provider لسه مجهزش
                var interceptor = sp.GetService<EntitySaveChangesInterceptor>();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                options.UseSqlServer(connectionString);

                // لا يضيف الـ Interceptor إلا إذا كان موجوداً فعلاً
                if (interceptor != null)
                {
                    options.AddInterceptors(interceptor);
                }
            });

            // 3. تسجيل الـ Repositories والـ Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 4. تسجيل الـ Infrastructure Services
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            // 5. تسجيل الـ Messaging (MassTransit)
            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    // تأكد إن الـ Configuration مش Null قبل القراءة
                    var host = configuration["RabbitMQ:Host"] ?? "localhost";
                    var user = configuration["RabbitMQ:Username"] ?? "guest";
                    var pass = configuration["RabbitMQ:Password"] ?? "guest";

                    cfg.Host(host, "/", h =>
                    {
                        h.Username(user);
                        h.Password(pass);
                    });
                });
            });

            services.AddScoped<IMessageBus, MessageBus>();

            return services;
        }
    }
}