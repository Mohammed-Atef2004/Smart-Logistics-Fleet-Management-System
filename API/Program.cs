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


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddApplication(); 
            builder.Services.AddInfrastructure(builder.Configuration); 
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
           
            var app = builder.Build();


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