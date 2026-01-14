using System.Reflection;
using Application.Common.Behaviors; // تأكد أن الفولدر ده موجود
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application; // تأكد أن الـ Namespace بسيط كدة

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // تسجيل MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(assembly);

            // لو الـ ValidationBehavior موجود، ضيف السطر ده
            // cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // تسجيل FluentValidation (الميثود دي محتاجة باكيج DependencyInjectionExtensions)
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}