using Asp.Versioning;
using MechanicShop.Application.Features.Identity;
using MechanicShop.Application.Features.Identity.Queries.GenerateTokens;
using MechanicShop.Application.Features.Identity.Queries.GetUserInfo;
using MechanicShop.Application.Features.Identity.Queries.RefreshTokens;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MechanicShop.API.Controllers
{
    [Route("identity")]
    [ApiVersionNeutral]
    public class IdentityController(ISender sender) : ApiController
    {
        [HttpPost("token/generate")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Generates an access and refresh token for a valid user.")]
        [EndpointDescription("Authenticates a user using provided credentials and returns a JWT token pair.")]
        [EndpointName("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] GenerateTokensQuery request,CancellationToken ct)
        {
            var result=await sender.Send(request);

            return result.Match(
           response => Ok(response),
           Problem);
        }

        [HttpPost("token/refresh-token")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Refreshes access token using a valid refresh token.")]
        [EndpointDescription("Exchanges an expired access token and a valid refresh token for a new token pair.")]
        [EndpointName("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenQuery request, CancellationToken ct)
        {
            var result = await sender.Send(request, ct);

            return result.Match(
           response => Ok(response),
           Problem);
        }
        [Authorize]
        [HttpGet("current-user/claims")]
        public async Task<IActionResult> GetCurrentUserInfo(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await sender.Send(new GetUserByIdQuery(userId), ct);

            return result.Match(
           response => Ok(response),
           Problem);
        }
    }
}
