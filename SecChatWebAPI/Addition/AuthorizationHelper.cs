using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecChatWebAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SecChatWebAPI.Addition
{
    public class AuthorizationHelper
    {       
        public static async Task<string> Authorization(string email, string password)
        {
            var passwordHasher = new PasswordHasher<User>();
            User? user = new ApplicationContext().Users
                .AsEnumerable()
                .Where(u => u.UserEmail == email &&
                (passwordHasher.VerifyHashedPassword(u, u.HashPassword, password) == PasswordVerificationResult.Success ||
                passwordHasher.VerifyHashedPassword(u, u.HashPassword, password) == PasswordVerificationResult.SuccessRehashNeeded))
                .FirstOrDefault();
            if (user is null) return null;
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Email, email)
            };
            var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.AddYears(1),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        public async Task Logout(WebApplication app)
        {

        }
        public static string GetUserId(HttpContext context)
        {
            var user = context.User.Claims.Where(s => s.Type.Split('/').Last() == "nameidentifier").First().Value;
            return user;
        }
    }
}
