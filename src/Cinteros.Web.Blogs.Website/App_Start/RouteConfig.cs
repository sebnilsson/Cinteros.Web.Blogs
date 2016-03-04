using System.Web.Mvc;
using System.Web.Routing;

namespace Cinteros.Web.Blogs.Website
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            routes.MapRoute(
                "Archive",
                "{year}/{month}",
                new { controller = "Blog", action = "Archive", },
                new { year = @"\d+", month = @"\d+", });

            routes.MapRoute("Search", "search", new { controller = "Blog", action = "Search", });

            routes.MapRoute("Tag", "tag", new { controller = "Blog", action = "Tag", });

            /*routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Blog", action = "Index", id = UrlParameter.Optional }
            );*/

            routes.MapRoute(
                name: "Info",
                url: "Info/{action}",
                defaults: new { controller = "Info", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Services",
                url: "Services/{action}",
                defaults: new { controller = "Services", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(name: "RSS", url: "rss", defaults: new { controller = "Services", action = "Rss" });

            routes.MapRoute(
                name: "CommunityRSS",
                url: "community-rss",
                defaults: new { controller = "Services", action = "CommunityRss" });

            routes.MapRoute(name: "Empty", url: string.Empty, defaults: new { controller = "Blog", action = "Index" });
        }
    }
}