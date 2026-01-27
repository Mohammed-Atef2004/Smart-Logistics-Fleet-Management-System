using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace API
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // 1. الوصول لملف الـ appsettings.json يدوياً
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 2. التأكد إن الـ Connection String مش بـ null
            if (string.IsNullOrEmpty(connectionString))
            {
                // لو السطر ده اتنفذ، يبقى المشكلة إن الـ EF مش عارف يقرأ ملف الـ JSON
                connectionString = "Server=.;Database=SLFM;Trusted_Connection=True;TrustServerCertificate=True";
            }

            builder.UseSqlServer(connectionString);

            // بنرجّع الـ DbContext فاضي من غير Interceptors عشان نعدي الـ Migration
            return new ApplicationDbContext(builder.Options);
        }
    }
}