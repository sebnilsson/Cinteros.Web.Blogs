using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Blaven;
using Blaven.DataSources.Blogger;
using Blaven.RavenDb;
using Raven.Client;

namespace Cinteros.Web.Blogs.Website
{
    public class MvcApplication : HttpApplication
    {
        private static FileSystemWatcher configWatcher;

        private static DateTime lastUnstaleIndexes;

        public static IDocumentStore DocumentStore { get; set; }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

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

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (custom.Equals("RavenDbStaleIndexes", StringComparison.InvariantCultureIgnoreCase))
            {
                bool staleIndexes = DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Any();
                if (!staleIndexes)
                {
                    lastUnstaleIndexes = DateTime.Now;
                    return string.Empty;
                }

                return string.Format("StaleIndexsFrom_{0}", lastUnstaleIndexes.Ticks);
            }

            return base.GetVaryByCustomString(context, custom);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            SetupBloggerViewController();
        }

        private static void SetupBloggerViewController()
        {
            DocumentStore = RavenDbHelper.GetDefaultDocumentStore();

            BlogService.InitInstance(DocumentStore);

            foreach (var setting in BlogService.Instance.Settings)
            {
                setting.BlogDataSource = new BloggerDataSource();
            }

            StartWatchConfig(AppSettingsService.BloggerSettingsPath);
        }

        private static void StartWatchConfig(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            configWatcher = new FileSystemWatcher(fileInfo.Directory.FullName, fileInfo.Name);

            configWatcher.Changed += new FileSystemEventHandler(ConfigWatcher_Changed);
            configWatcher.EnableRaisingEvents = true;
        }

        private static void ConfigWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            HttpRuntime.UnloadAppDomain();
        }
    }
}