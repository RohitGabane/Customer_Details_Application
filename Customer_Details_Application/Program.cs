using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Customer_Details_Application.Data;
namespace Customer_Details_Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.Services.AddDbContext<CustomerContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("CustomerContext") ?? throw new InvalidOperationException("Connection string 'CustomerContext' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Customers}/{action=Index}");

            app.Run();
        }
    }
}
