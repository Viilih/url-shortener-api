namespace UrlShortner.Api.Entities;

public class ShortUrl
{
    public Guid Id { get; set; }
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortenedUrl { get; set; } = string.Empty;
    public string UniqueCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}