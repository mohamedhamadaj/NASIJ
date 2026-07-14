using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TMAProject.Comomn.DBInitilizer;
using TMAProject.DataAccess;
using TMAProject.Models.Entities;
using TMAProject.Repository.Implementations;
using TMAProject.Repository.Interfaces;
using TMAProject.Services.Implementations;
using TMAProject.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TMAProject
{
    public static class AppConfiguration
    {
        public static void Registerconfig(this IServiceCollection services)
        {
            // Register your configuration settings here
            // Example: services.Configure<MySettings>(configuration.GetSection("MySettings"));
            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.Password.RequireDigit = true;
                option.Password.RequiredLength = 8;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = true;
                option.Password.RequireLowercase = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login"; // Default login path
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Default access denied path
            });

            services.AddScoped<IImageService, ImageService>();

            // Repositories
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IColorRepository, ColorRepository>();
            services.AddScoped<ISizeRepository, SizeRepository>();

            // Services
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IDBInitilizer, DBInitilizer>();


        }
    }
}
