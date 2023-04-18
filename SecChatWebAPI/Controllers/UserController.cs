using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecChatWebAPI.Addition;
using SecChatWebAPI.Models;
using SecChatWebAPI.Requests;
using SecChatWebAPI.Responses;
using SecChatWebAPI.Services;

namespace SecChatWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FileManager fileManager;

        public UserController(FileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        [HttpGet(Name = "GetUsersBySubStringId")]
        async public Task<List<UserResponse>> GetUsersBySubStringId(string? UserId)
        {
            var list = DbHelper.GetUsersBySubStringId(UserId).Select<User, UserResponse>(u =>
            {
                {
                    var user = new UserResponse()
                    {
                        UserId = u.UserId,
                        UserEmail = u.UserEmail,
                        UserName = u.UserName,
                        UserImg = u.UserImg
                    };
                    return user;
                }
            }).ToList();
            return list;
        }

        [HttpPut(Name = "UpdateUser")]
        [Authorize]
        async public Task<User> UpdateUser([FromForm] UserRequest request)
        {
            using (var db = new ApplicationContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.UserEmail == request.UserId);
                user.UserName = request.UserName;
                if (request.formFile != null)
                {
                    user.UserImg = "http://securitychat.ru" + await fileManager.AddFile(request.formFile);
                }
                return user;
            }   
        }


        [HttpGet(Name = "GetUserByEmail")]
        async public Task<UserResponse> GetUserByEmail(string UserEmail)
        {
            var u = await DbHelper.GetUserByEmail(UserEmail);
            var newUser = new UserResponse() { UserId = u.UserId, UserEmail = u.UserEmail, UserImg = u.UserImg, UserName = u.UserName };
            return newUser;
        }

        [HttpPost(Name = "GetChatsUser")]
        async public Task<List<Chat>> GetChatsUser([FromForm] UserRequest user)
        {
            return DbHelper.GetChatsUser(user.UserEmail);
        }

        [HttpGet(Name = "GetChatsUserByEmail")]
        [Authorize]
        async public Task<IEnumerable<Chat>> GetChatsUserByEmail(string UserEmail)
        {
            var usermain = await DbHelper.GetUserByEmail(UserEmail);
            var list = DbHelper.GetChatsUser(usermain.UserId);
            for (int i = 0; i < list.Count(); i++)
            {
                var chat = list[i];
                if (chat.isDialog == true)
                {
                    var altUser = DbHelper.GetAltUser(chat, usermain.UserId);
                    chat.ChatName = altUser.UserName;
                    chat.ChatImg = altUser.UserImg;
                }
            }
            return list.OrderBy(x => x.ChatUpdateTime).Reverse();
        }
    }
}
