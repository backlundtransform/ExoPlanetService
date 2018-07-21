using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace ExoPlanetHunter.Web.Config
{
    public class AddOdataParameters : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters != null)
            {
                if (operation.Parameters.FirstOrDefault()?.Name == "opts")
                {
                    operation.Parameters = new List<IParameter>();
                    operation.Parameters.Add(new NonBodyParameter() { Name = "$filter", In = "query" });

                    operation.Parameters.Add(new NonBodyParameter() { Name = "$top", In = "query" });
                    operation.Parameters.Add(new NonBodyParameter() { Name = "$orderby", In = "query" });

                    operation.Parameters.Add(new NonBodyParameter() { Name = "$skip", In = "query" });
                   
                }
            }
        }
    }
}