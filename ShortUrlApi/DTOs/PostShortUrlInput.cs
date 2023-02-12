namespace ShortUrlApi.DTOs
{
    public class PostShortUrlInput
    {
        public string Origin { get; set; }
        public DateTime Expiration { get; set; }
    }
}
