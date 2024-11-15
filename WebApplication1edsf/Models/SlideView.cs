namespace WebApplication1edsf.Models
{
    public class SlideView
    {

        public string Type { get; set; } = "slide";
        public string Title { get; set; }
        public string Content { get; set; }
        public string Forms { get; set; } = "";
        public SlideView(string title, string content) {
            Title = title;
            Content = content;

        }
        public SlideView(string type, string title, string content)
        {
            Type = type;
            Title = title;
            Content = content;

        }
    }
}
