using MediatR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UrlShortner.Api.Database;
using UrlShortner.Api.Entities;

namespace UrlShortner.Api.Features.ShortenUrl.GetShortenedUrl;

public sealed record GetShortUrlQuery(string ShortUrlCode) : IRequest<GetShortUrlResult>;

public sealed record GetShortUrlResult(string LongUrl);

public sealed class GetShortUrlHandler : IRequestHandler<GetShortUrlQuery, GetShortUrlResult>
{
    private readonly ApplicationDbContext _context;
    private readonly IDatabase _redisDb;

    public GetShortUrlHandler(ApplicationDbContext context, IConnectionMultiplexer connectionMultiplexer)
    {
        _context = context;
        _redisDb = connectionMultiplexer.GetDatabase();
    }

    public async Task<GetShortUrlResult> Handle(GetShortUrlQuery query, CancellationToken cancellationToken)
    {
        var cachedUrl = await _redisDb.StringGetAsync(query.ShortUrlCode);

        if (!cachedUrl.IsNullOrEmpty)
        {
            return new GetShortUrlResult(cachedUrl.ToString());
        }

        var urlEntity = await GetUrlAsync(query.ShortUrlCode, cancellationToken);
        
        if (urlEntity == null)
        {
            throw new ArgumentException("Shortened Url not found");
        }
        await _redisDb.StringSetAsync(query.ShortUrlCode, urlEntity.OriginalUrl, TimeSpan.FromDays(1));
        return new GetShortUrlResult(urlEntity.OriginalUrl);
    }

    private async Task<ShortUrl?> GetUrlAsync(string urlCode, CancellationToken cancellationToken)
    {
        return await _context.ShortUrls.SingleOrDefaultAsync(d => string.Equals(d.UniqueCode, urlCode), cancellationToken);
    } 
}