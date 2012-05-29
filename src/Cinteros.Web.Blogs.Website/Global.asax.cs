using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Blaven;
using Raven.Client;
using Raven.Client.Document;

namespace Cinteros.Web.Blogs.Website {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes) {
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
                new { year = @"\d+", month = @"\d+", }
            );

            routes.MapRoute(
                "Search",
                "search",
                new { controller = "Blog", action = "Search", }
            );

            routes.MapRoute(
                "Tag",
                "tag",
                new { controller = "Blog", action = "Tag", }
            );

            /*routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Blog", action = "Index", id = UrlParameter.Optional }
            );*/

            routes.MapRoute(
                name: "Info",
                url: "Info/{action}",
                defaults: new { controller = "Info", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Services",
                url: "Services/{action}",
                defaults: new { controller = "Services", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Empty",
                url: "",
                defaults: new { controller = "Blog", action = "Index" }
            );
        }

        private static IDocumentStore DocumentStore { get; set; }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            SetupBloggerViewController();
        }

        private static void SetupBloggerViewController() {
            // Init Raven
            MvcApplication.DocumentStore = new DocumentStore() {
                ApiKey = AppSettingsService.RavenDbStoreApiKey,
                Url = AppSettingsService.RavenDbStoreUrl,
            };
            MvcApplication.DocumentStore.Initialize();

            // Init Blaven config
            _bloggerSettingsFilePath = HttpContext.Current.Server.MapPath(AppSettingsService.BloggerSettingsPath);
            StartWatchConfig(_bloggerSettingsFilePath);
            
            var service = GetBlogService();
            service.Refresh();

            Raven.Client.Indexes.IndexCreation.CreateIndexes(
                typeof(Blaven.RavenDb.Indexes.BlogPostsOrderedByCreated).Assembly, MvcApplication.DocumentStore);
        }

        private static string _bloggerSettingsFilePath;

        internal static BlogService GetBlogService() {
            var config = new BlogServiceConfig(_bloggerSettingsFilePath) {
                DocumentStore = MvcApplication.DocumentStore,
            };
            var service = new BlogService(config);
            return service;
        }

        private static FileSystemWatcher _configWatcher;
        private static void StartWatchConfig(string filePath) {
            var fileInfo = new FileInfo(filePath);
            _configWatcher = new FileSystemWatcher(fileInfo.Directory.FullName, fileInfo.Name);

            _configWatcher.Changed += new FileSystemEventHandler(ConfigWatcher_Changed);
            _configWatcher.EnableRaisingEvents = true;
        }

        private static void ConfigWatcher_Changed(object sender, FileSystemEventArgs e) {
            HttpRuntime.UnloadAppDomain();
        }
    }
}