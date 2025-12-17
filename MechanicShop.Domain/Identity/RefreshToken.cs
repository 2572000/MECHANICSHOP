using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Identity
{
    public sealed class RefreshToken:AuditableEntity
    {
        public DateTimeOffset ExpiresOnUts { get; }
        public string? Token { get;}
        public string? UserId { get;}

        private RefreshToken()
        {
            
        }

        private RefreshToken(Guid id,string token,string userId,DateTimeOffset expiresOnUts)
            :base(id)
        {
            Token = token;
            UserId = userId;
            ExpiresOnUts = expiresOnUts;
        }


        public static Result<RefreshToken> Create(Guid id,string token,string userId,DateTimeOffset expiresOnUts)
        {
            if (id == Guid.Empty)
            {
                return RefreshTokenErrors.IdRequired;
            }
            if (string.IsNullOrWhiteSpace(token))
            {
                return RefreshTokenErrors.TokenRequired;
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                return RefreshTokenErrors.UserIdRequired;
            }
            if (expiresOnUts <= DateTimeOffset.UtcNow)
            {
                return RefreshTokenErrors.ExpiryInvalid;
            }
            return new RefreshToken(id, token, userId, expiresOnUts);
        }

    }
}
