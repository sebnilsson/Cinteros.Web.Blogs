namespace Cinteros.Web.Blogs.Website.Models {
    public class MenuItem {
        public MenuItem(string text, string url, string @class = null) {
            this.Text = text;
            this.Url = url;
            this.Class = @class ?? string.Empty;
        }

        public string Id { get; set; }
        public int Index { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public string Class { get; set; }
    }
}