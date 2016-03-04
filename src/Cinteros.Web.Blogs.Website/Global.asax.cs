using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cinteros.Web.Blogs.Website
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            Parallel.Invoke(
                () =>
                    {
                        ViewEngines.Engines.Clear();
                        ViewEngines.Engines.Add(new RazorViewEngine());
                    },
                () =>
                    {
                        RegisterGlobalFilters(GlobalFilters.Filters);
                    },
                () =>
                    {
                        AreaRegistration.RegisterAllAreas();

                        RouteConfig.RegisterRoutes(RouteTable.Routes);
                    },
                BlavenConfig.Init);
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (custom.Equals("RavenDbLastUpdate", StringComparison.InvariantCultureIgnoreCase))
            {
                var statistics = BlavenConfig.DocumentStore.DatabaseCommands.GetStatistics();
                return $"StaleIndexsFrom_{statistics.LastDocEtag}";
            }

            if (custom.Equals("StatusDescription", StringComparison.InvariantCultureIgnoreCase))
            {
                return this.Response.StatusDescription;
            }

            return base.GetVaryByCustomString(context, custom);
        }
    }
}