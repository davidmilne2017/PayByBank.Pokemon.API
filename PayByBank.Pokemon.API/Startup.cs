using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PayByBank.Pokemon.Common.Interfaces;
using PayByBank.Pokemon.Infrastructure.Adapters;
using PayByBank.Pokemon.Infrastructure.Repositories;
using PayByBank.Pokemon.Services.Services;
using System;
using System.IO;
using System.Reflection;

namespace PayByBank.Pokemon.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
                services.AddSwaggerGen(c =>
                    {
                        c.SwaggerDoc("v1", new OpenApiInfo { Title = "PokemonAPI", Version = "v1" });
                        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                        c.IncludeXmlComments(xmlPath);
                    })
                .AddSingleton(Configuration)
                .AddTransient<IActionContextAccessor, ActionContextAccessor>()
                .AddTransient<IPokemonHttpRepository, PokemonHttpRepository>()
                .AddTransient<ITranslationHttpRepository, TranslationHttpRepository>()
                .AddTransient<IPokemonConverterAdapter, PokemonConverterAdapter>()
                .AddTransient<IPokemonService, PokemonService>()
                .AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger()
               .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pokemon API");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
