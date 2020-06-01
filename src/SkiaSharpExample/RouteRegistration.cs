using Cofoundry.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkiaSharpExample
{
    public class RouteRegistration : IRouteRegistration
    {
        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapRazorPages();
        }
    }
}
