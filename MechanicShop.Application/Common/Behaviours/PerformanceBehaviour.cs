using MechanicShop.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MechanicShop.Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse>(ILogger<TRequest> logger,
        IIdentityService identityService, IUser user)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<TRequest> _logger = logger;
        private readonly IIdentityService _identityService = identityService;
        private readonly IUser _user = user;
        private readonly Stopwatch _timer = new Stopwatch();

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            _timer.Start();

            var response =await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = _user.Id ?? string.Empty;
                var userName = string.Empty;

                if (!string.IsNullOrEmpty(userId))
                {
                    userName=await _identityService.GetUserNameAsync(userId);
                }
                _logger.LogWarning("Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                    requestName, elapsedMilliseconds, userId, userName, request);
            }
            return response;
        }
    }
}
