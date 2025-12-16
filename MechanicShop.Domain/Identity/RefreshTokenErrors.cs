using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Identity
{
    public static class RefreshTokenErrors
    {
        public static Error IdRequired =>
            Error.Validation("RefreshToken.IdRequired", "The refresh token ID is required.");
        public static Error TokenRequired =>
            Error.Validation("RefreshToken.TokenRequired", "The refresh token is required.");
        public static Error UserIdRequired =>
            Error.Validation("RefreshToken.UserIdRequired", "The user ID is required.");

        public static Error ExpiryInvalid =>
            Error.Validation("RefreshToken.ExpiryInvalid", "The refresh token expiry date is invalid.");
    }
}
