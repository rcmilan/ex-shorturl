namespace ShortUrlApi.DTOs
{
    public class PostShortUrlInput
    {
        public string Target { get; set; }
        public DateTime Expiration { get; set; }

        public bool IsValidUri() =>
            Uri.TryCreate(this.Target, UriKind.Absolute, out Uri uriResult) ||
            uriResult.Scheme != Uri.UriSchemeHttp &&
            uriResult.Scheme != Uri.UriSchemeHttps;
    }
}