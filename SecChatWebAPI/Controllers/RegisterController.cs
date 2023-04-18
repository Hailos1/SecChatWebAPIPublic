using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecChatWebAPI.Addition;
using SecChatWebAPI.Models;
using SecChatWebAPI.Responses;
using System.Security;

namespace SecChatWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {     
        [HttpPost(Name = "Register")]
        public async Task<RegisterResponse> Register([FromForm] User userin)
        {
            var sender = new EmailSender();
            var code = new Random().Next(111111, 999999).ToString();
            await sender.SendCode(userin.UserEmail, code);
            var answer = new RegisterResponse() { Answer = true, HashCode = TryHashCode.HashPassword(code) };
            return answer;
        }

        [HttpPost(Name = "TryCode")]
        public async Task<RegisterResponse> TryCode([FromForm] User userin, [FromForm] string code, [FromForm] string hashCode)
        {
            if (TryHashCode.VerifyHashedPassword(hashCode, code))
            {
                return await ConfirmRegister(userin);
            }
            return new RegisterResponse() { Answer = false, Message = "Неверный email код" }; ;
        }

        private static async Task<RegisterResponse> ConfirmRegister(User userin)
        {
            var db = new ApplicationContext();
            if (db.Users.AsEnumerable().Where(u => u.UserId == userin.UserId).FirstOrDefault() == null && db.Users.AsEnumerable().Where(u => u.UserEmail == userin.UserEmail).FirstOrDefault() == null)
            {
                var user = new User()
                {
                    UserId = userin.UserId.ToLower(),
                    UserEmail = userin.UserEmail,
                    UserName = userin.UserName,
                    UserImg = userin.UserImg,
                    UserCreated = DateTime.UtcNow
                };

                user.HashPassword = new PasswordHasher<User>().HashPassword(user, userin.HashPassword);
                await db.Users.AddAsync(user);
                db.SaveChanges();
                return new RegisterResponse() { Answer = true};
            }
            else
            {
                return new RegisterResponse() { Answer = false, Message = "Пользователь уже зарегистрирован" };
            }
        }
    }
}
