using Microsoft.EntityFrameworkCore;
using SwitchManagment.API.AutoMapper;
using SwitchManagment.API.Db;
using SwitchManagment.API.SwitchService.Implementations;
using SwitchManagment.API.SwitchService.Interfaces;
/*
using SwitchManagment.API.Repository.Interfaces;
using SwitchManagment.API.Repository.SqlLite;
using SwitchManagment.API.Repository.SqlLite.Implementations;
*/

namespace SwitchManagment.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region AddServices
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //builder.Services.AddScoped<ISwitchRepository, SwitchRepository>();

            builder.Services.AddDbContext<ApplicationContext>(opts => 
                opts.UseSqlite(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddAutoMapper(ca => ca.AddProfile<AutoMapperProfile>());
            builder.Services.AddTransient<ISwitchService, SwitchServiceHPComware5>();
            builder.Services.AddDataProtection();
            #endregion

            #region UseMiddleware
            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.MapOpenApi("/swagger/openapi/{documentName}.json");
                app.MapOpenApi();
                app.UseSwaggerUI(opts =>
                    opts.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}
