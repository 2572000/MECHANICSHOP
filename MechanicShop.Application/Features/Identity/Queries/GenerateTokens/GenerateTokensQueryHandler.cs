using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Identity.Queries.GenerateTokens
{
    public class GenerateTokensQueryHandler(ILogger<GenerateTokensQueryHandler> logger, 
        IIdentityService identityService,
        ITokenProvider tokenProvider)
        :IRequestHandler<GenerateTokensQuery, Result<TokenResponse>>
    {
        private readonly ILogger<GenerateTokensQueryHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;
        private readonly ITokenProvider _tokenProvider = tokenProvider;

        public async Task<Result<TokenResponse>> Handle(GenerateTokensQuery request, CancellationToken ct)
        {
            var userResponse= await _identityService.AuthenticateAsync(request.Email, request.Password);

            if (userResponse.IsError)
            {
                return userResponse.Errors!;
            }

            var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(userResponse.Value, ct);

            if (generateTokenResult.IsError)
            {
                _logger.LogError("Generate token error occurred: {ErrorDescription}", generateTokenResult.TopError.Description);
                return generateTokenResult.Errors!;
            }

            return generateTokenResult.Value!;
        }
    }
}
