using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecChatWebAPI.Addition;
using SecChatWebAPI.Requests;
using System.Text;

namespace SecChatWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {     

        [HttpPost]
        async public Task<string> Authorization([FromForm] AuthRequest info)
        {
            return await AuthorizationHelper.Authorization(info.UserEmail, info.UserPassword);
        }
    }
}
