namespace HackerNewsApiWeb.Models
{
    public class HackerNewsItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string By { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }
        public string[] Kids { get; set; }
        public string Type { get; set; }
    }
}
