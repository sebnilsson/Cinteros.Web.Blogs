using System;
using System.Configuration;
using System.Linq;

using Blaven;
using Blaven.BlogSources.Blogger;
using Blaven.Data.RavenDb2;
using Blaven.Synchronization;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;

namespace Cinteros.Web.Blogs.Website
{
    public static class BlavenConfig
    {
        internal static IDocumentStore DocumentStore { get; private set; }

        internal static readonly Lazy<BlogSetting[]> BlogSettingsLazy =
            new Lazy<BlogSetting[]>(() => AppSettingsFactory.GetBlogSettings().ToArray());

        public static Func<BlogQueryService> BlogQueryServiceFactory
        {
            get
            {
                return () => GetBlogQueryService(DocumentStore);
            }
        }

        public static Func<BlogSyncService> BlogSyncServiceFactory
        {
            get
            {
                return () => GetBlogSyncService(DocumentStore);
            }
        }

        public static void Init()
        {
            DocumentStore = GetDocumentStore();
        }

        private static BlogQueryService GetBlogQueryService(IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException(nameof(documentStore));
            }

            var repository = new RavenDbRepository(DocumentStore);

            var blogQuery = new BlogQueryService(repository, BlogSettingsLazy.Value);
            return blogQuery;
        }

        private static BlogSyncService GetBlogSyncService(IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException(nameof(documentStore));
            }

            var blogSource = AppSettingsFactory.BuildBlogSource((username, password) => new BloggerBlogSource(password));

            var dataStorage = new RavenDbDataStorage(documentStore);

            var syncConfig = new BlogSyncConfiguration(blogSource, dataStorage, blogSettings: BlogSettingsLazy.Value);

            syncConfig.TransformersProvider.Transformers.Clear();

            var blogSync = new BlogSyncService(syncConfig);
            return blogSync;
        }

        private static IDocumentStore GetDocumentStore()
        {
            string appSetting = ConfigurationManager.AppSettings["RAVENHQ_CONNECTION_STRING"];

            var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(appSetting);
            parser.Parse();

            string apiKey = parser.ConnectionStringOptions.ApiKey;
            string url = parser.ConnectionStringOptions.Url;

            var documentStore = new DocumentStore { ApiKey = apiKey, Url = url };

            RavenDbInitializer.Initialize(documentStore);

            return documentStore;
        }
    }
}