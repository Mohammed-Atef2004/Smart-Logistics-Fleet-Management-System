using Infrastructure;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using API.Middleware;
using Application; // تأكد إن الـ Namespace ده موجود عشان AddApplication تشتغل

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- 1. تسجيل الخدمات (Dependency Injection) ---

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // نداء طبقات المشروع (مرة واحدة فقط لكل طبقة)
            builder.Services.AddApplication(); // هيسجل الـ Handlers والـ Validators
            builder.Services.AddInfrastructure(builder.Configuration); // هيسجل الداتابيز والـ MassTransit

            var app = builder.Build();

            // --- 2. ترتيب الـ Pipeline (الميدل وير) ---

            // الميدل وير بتاعنا لازم يكون أول واحد
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}