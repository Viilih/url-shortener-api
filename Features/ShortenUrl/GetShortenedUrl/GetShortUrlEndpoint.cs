using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortner.Api.Features.ShortenUrl.GetShortenedUrl;


[ApiController]
[Route("[controller]")]
public class GetShortUrlEndpoint
{
    private readonly IMediator _mediator;

    public GetShortUrlEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("/{uniqueCode}")]
    public async Task<IResult> GetUrlAsync(string uniqueCode)
    {
        if (string.IsNullOrEmpty(uniqueCode))
        {
            throw new ArgumentNullException(nameof(uniqueCode));
        }
        
        var query = new GetShortUrlQuery(uniqueCode);

        var result = await _mediator.Send(query);
        
        return Results.Redirect(result.LongUrl);
    }
}