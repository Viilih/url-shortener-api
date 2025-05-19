using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortner.Api.Features.ShortenUrl.CreateShortenUrl;

[ApiController]
[Route("url")]
public class CreateShortUrlEndpoint : ControllerBase
{
    
    private readonly IMediator _mediator;

    public CreateShortUrlEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("/create-short-url")]
    public async Task<IResult> CreateShortUrl([FromBody] CreateShortUrlRequest request)
    {
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var _))
        {
            return Results.BadRequest(BadRequest("Invalid url"));
        }
        
        var command = new CreateShortUrlCommand(request.Url);
        
        var result = await _mediator.Send(command);

        return Results.Ok(result.ShortenedUrl);
    }
}