using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecChatWebAPI.Addition;
using SecChatWebAPI.Models;
using System.Text.Json;

namespace SecChatWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        //[FromForm] MessageRequest request
        [HttpGet(Name = "GetMessages")]
        [Authorize]
        async public Task<IEnumerable<MessageInfo>> GetMessages(int ChatId, int start, int end)
        {
            var db = new ApplicationContext();
            var s = DbHelper.GetMessages(Convert.ToInt32(ChatId), start, end);
            var messageInfoEnum = s.Select(message =>
            {
                var user = db.Users.Where(v => v.UserId == message.MessageUserId).First();
                var pathFile = DbHelper.GetMessageFile(db.Messages.AsEnumerable().Where(m => m.MessageId == message.MessageId).FirstOrDefault().MessageAdditionsId);
                return new MessageInfo()
                {
                    UserId = message.MessageUserId,
                    UserName = user.UserName,
                    UserImg = user.UserImg,
                    Message = TryHashCode.DecryptSha256(message.MessageText),
                    MessageId = message.MessageId,
                    TimeSend = message.MessageSendTime.ToString(),
                    pathsAddition = pathFile
                };
            });
            return messageInfoEnum;
        }
    }
}
