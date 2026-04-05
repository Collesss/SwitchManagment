using Microsoft.AspNetCore.Authentication.Negotiate;
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

            #region AddServices
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddProblemDetails();
            
            //builder.Services.AddScoped<ISwitchRepository, SwitchRepository>();

            builder.Services.AddDbContext<ApplicationContext>(opts => 
                opts.UseSqlite(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddAutoMapper(ca => ca.AddProfile<AutoMapperProfile>());
            builder.Services.AddTransient<ISwitchService, SwitchServiceHPComware5>();
            builder.Services.AddDataProtection(opts => opts.ApplicationDiscriminator = "SwitchManagmentApi");

            builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                .AddNegotiate();
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

            app.UseStatusCodePages();
            app.UseExceptionHandler();

            app.UseAuthorization();

            //app.UseDeveloperExceptionPage();
            /*
            app.UseDeveloperExceptionPage(); -- используется по умолчанию не форматированный вывод.

            builder.Services.AddProblemDetails(); -- выдаёт форматированный подробный вывод.

            app.UseExceptionHandler(); -- вместе с builder.Services.AddProblemDetails(); даёт сокращённый форматированный вывод.

            app.UseStatusCodePages(); -- пока не совсем понял, но при не правильном маршруте выдаёт пустой ответ в отличии от app.UseDeveloperExceptionPage();

            builder.Services.AddProblemDetails();
            app.UseExceptionHandler();
            app.UseStatusCodePages();
            если использовать три команды выше, то будет выдаваться сокращённый форматированный ответ даже на ошибки маршрута, без app.UseStatusCodePages(); ошибки марштура не дают ответа.
            */


            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}
