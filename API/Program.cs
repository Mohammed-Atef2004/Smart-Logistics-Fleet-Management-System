using Application.Features.Driver.Commands.HireDriver;
using Infrastructure.Persistence;
using Infrastructure.Presistence.Data;
using Microsoft.EntityFrameworkCore;
using WebApi.Middlewares;

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
            builder.Services.AddLogging();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication(); 
            builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(HireDriverCommand).Assembly));

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