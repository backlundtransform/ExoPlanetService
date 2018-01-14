using System;

using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OData.Edm;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ExoPlanetHunter.Pocos;
using Swashbuckle.AspNetCore.Swagger;

namespace ExoPlanetHunter.Web
{
    public class Startup
    {

        private static IEdmModel GetEdmModel(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);
            builder.EntitySet<Planet>("Planets");
            builder.EntitySet<Star>("Stars");
            return builder.GetEdmModel();
        }
      


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder();
            services.AddMvc();
            services.AddOData();

          
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "ExoPlanet API", Version = "v1" });
            });
            Service.Logic.Startup(services);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseSwagger();
            

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExoPlanet API V1");
            });

            IEdmModel model = GetEdmModel(app.ApplicationServices);



            app.UseMvc(routeBuilder =>
            {
                routeBuilder.MapODataServiceRoute("odata", "odata", model);

              
                routeBuilder.EnableDependencyInjection();
            });
          
        }
    }
}
