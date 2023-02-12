namespace ShortUrlApi.Services
{
    public interface IShortUrlService
    {
        string Generate();
    }

    public class ShortUrlService : IShortUrlService
    {
        public string Generate()
        {
            var characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            return Nanoid.Nanoid.Generate(characters, 10);
        }
    }
}
