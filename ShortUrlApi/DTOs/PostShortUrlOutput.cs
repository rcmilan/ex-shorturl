namespace ShortUrlApi.DTOs
{
    public class PostShortUrlOutput
    {
        public PostShortUrlOutput(string newUrl)
        {
            NewUrl = newUrl;
        }

        public string NewUrl { get; }
    }
}
