using FCG_Libraries.Api.Middlewares;
using FCG_Libraries.Application.Shared;
using FCG_Libraries.Infrastructure.Shared;
using FCG_Libraries.Infrastructure.Shared.Context;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FCG_Libraries.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
           // builder.WebHost.UseUrls("http://0.0.0.0:80");

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddApplicationServices();

            builder.Services.AddHttpClient("GamesApi", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["Services:GamesApi"]!);
            });

            builder.Services.AddHttpClient("UsersApi", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["Services:UsersApi"]!);
            });

            builder.Services.AddControllers();
                        
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

           /* builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(80); 
            });*/
            
            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseMiddleware<CorrelationIdMiddleware>();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

                var retries = 5;
                while (retries > 0)
                {
                    try
                    {
                        db.Database.Migrate();
                        break;
                    }
                    catch
                    {
                        retries--;
                        Thread.Sleep(2000); 
                    }
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();           

            app.Run();
        }
    }
}
