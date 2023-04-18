using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SecChatWebAPI.Addition
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            //var id = connection.User.Claims.Where(s => s.Type.Split('/').Last() == "nameidentifier").First().Value;
            //return id;
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
