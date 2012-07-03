namespace Cinteros.Web.Blogs.Website.Models {
    public class MenuItem {
        public MenuItem(string text, string url, string @class = null) {
            this.Text = text;
            this.Url = url;
            this.Class = @class ?? string.Empty;
        }

        public string Text { get; set; }
        public string Url { get; set; }
        public string Class { get; set; }
    }
}