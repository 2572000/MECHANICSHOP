using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity;
using MechanicShop.Application.Features.Identity.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MechanicShop.Infrastructure.Identity
{
    public class TokenProvider(IConfiguration configuration, IAppDbContext context) : ITokenProvider
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IAppDbContext _context = context;

        public async Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default)
        {
            var tokenResult = await CreateAsync(user, ct);

            if (tokenResult.IsError)
            {
                return tokenResult.Errors!;
            }

            return tokenResult.Value;
        }



        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            throw new NotImplementedException();
        }

        private async Task<Result<TokenResponse>> CreateAsync(AppUserDto user, CancellationToken ct)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var issuer = jwtSettings["Issuer"]!;
            var audience = jwtSettings["Audience"]!;
            var key = jwtSettings["Secret"]!;

            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenExpirationInMinutes"]!));


            // Package=> IdentityModel.Tokens.Jwt
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.UserId!),
                new (JwtRegisteredClaimNames.Email, user.Email!),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new(ClaimTypes.Role, role));
            }

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
               new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
               SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(descriptor);

            var oldRefreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == user.UserId)
            .ExecuteDeleteAsync(ct);

            var refreshTokenResult = RefreshToken.Create(
               Guid.NewGuid(),
               GenerateRefreshToken(),
               user.UserId,
               DateTime.UtcNow.AddDays(7));

            if (refreshTokenResult.IsError)
            {
                return refreshTokenResult.Errors!;
            }

            var refreshToken = refreshTokenResult.Value;

            _context.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync(ct);

            return new TokenResponse
            {
                AccessToken = tokenHandler.WriteToken(securityToken),
                RefreshToken = refreshToken.Token,
                ExpiresOnUtc = expires
            };
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
