using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

using Blaven;

namespace Cinteros.Web.Blogs.Website.Models
{
    public class BlogListViewModel
    {
        public BlogListViewModel(IQueryable<BlogPost> posts, int pageSize, int pageIndex, ControllerContext context)
        {
            this.FriendlyPageIndex = pageIndex + 1;

            this.Posts = posts.Paged(pageSize, pageIndex).ToList();

            this.HasNewerItems = (pageIndex > 0);

            this.HasOlderItems = posts.Paged(pageSize, pageIndex + 1).Any();

            int olderItemsIndex = this.FriendlyPageIndex + 1;
            int? newerItemsIndex = this.FriendlyPageIndex - 1;
            if (newerItemsIndex <= 1)
            {
                newerItemsIndex = null;
            }

            var olderItemRouteValues = new RouteValueDictionary(context.RouteData.Values);
            olderItemRouteValues["page"] = olderItemsIndex;

            var newerItemRouteValues = new RouteValueDictionary(context.RouteData.Values);
            newerItemRouteValues["page"] = newerItemsIndex;

            string t = context.HttpContext.Request.QueryString["t"];
            if (!string.IsNullOrWhiteSpace(t))
            {
                olderItemRouteValues["t"] = t;
                newerItemRouteValues["t"] = t;
            }
            string q = context.HttpContext.Request.QueryString["q"];
            if (!string.IsNullOrWhiteSpace(q))
            {
                olderItemRouteValues["q"] = q;
                newerItemRouteValues["q"] = q;
            }

            string currentAction = Convert.ToString(context.Controller.ValueProvider.GetValue("action").RawValue);

            var urlHelper = new UrlHelper(context.RequestContext);

            this.NewerItemsUrl = urlHelper.Action(currentAction, newerItemRouteValues);
            this.OlderItemsUrl = urlHelper.Action(currentAction, olderItemRouteValues);
        }

        public int FriendlyPageIndex { get; }

        public bool HasNewerItems { get; private set; }

        public bool HasOlderItems { get; private set; }

        public string NewerItemsUrl { get; private set; }

        public string OlderItemsUrl { get; private set; }

        public IReadOnlyCollection<BlogPost> Posts { get; private set; }
    }
}