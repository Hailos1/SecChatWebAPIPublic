using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecChatWebAPI.Models;
using SecChatWebAPI.Responses;

namespace SecChatWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {

        [HttpPost]
        [Authorize]
        public async Task<SettingsResponse> SaveSettings([FromForm] string userid, [FromForm] string name, [FromForm] string icon)
        {
            using (var db = new ApplicationContext())
            {
                var user = db.Users.First(x => x.UserId == userid);
                user.UserName = name;
                user.UserImg = icon;
                await db.SaveChangesAsync();
                return new SettingsResponse() { Answer = true };
            }
        }
    }
}
