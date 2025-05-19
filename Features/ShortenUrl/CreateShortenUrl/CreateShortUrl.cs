using MediatR;
using Microsoft.EntityFrameworkCore;
using UrlShortner.Api.Database;
using UrlShortner.Api.Entities;

namespace UrlShortner.Api.Features.ShortenUrl.CreateShortenUrl;

public sealed record CreateShortUrlCommand(string Url) : IRequest<CreateShortUrlResult>;

public sealed record CreateShortUrlResult(string ShortenedUrl); 

public sealed class CreateShortUrlHandler : IRequestHandler<CreateShortUrlCommand, CreateShortUrlResult>
{
    private readonly ApplicationDbContext _context;
    private const int MaxLength = 7;
    private const string AllLettersAndNumbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public CreateShortUrlHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<CreateShortUrlResult> Handle(CreateShortUrlCommand request,
        CancellationToken cancellationToken)
    {
        
        var uniqueCode = await GenerateUniqueCode();
        var newUrl = $"https://tiny-link/{uniqueCode}";
        var shortenedUrl = new ShortUrl
        {
            UniqueCode = uniqueCode,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OriginalUrl = request.Url,
            ShortenedUrl = newUrl

        };
        
        await _context.ShortUrls.AddAsync(shortenedUrl, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new CreateShortUrlResult(shortenedUrl.ShortenedUrl);
        
    }
    
    private async Task<string> GenerateUniqueCode()
    {
        Random random = new();

        var chars = new char[MaxLength];

        while (true)
        {
            for (int i = 0; i < MaxLength; i++)
            {
                var randomIndex = random.Next(AllLettersAndNumbers.Length);

                chars[i] = AllLettersAndNumbers[randomIndex];
            }

            string uniqueCode = new string(chars);

            if (!await IsUniqueCodeExist(uniqueCode))
            {
                return uniqueCode;
            }
        }
    }

    private async Task<bool> IsUniqueCodeExist(string uniqueCode) =>
        await _context.ShortUrls
            .AnyAsync(d => string.Equals(d.UniqueCode, uniqueCode));
}