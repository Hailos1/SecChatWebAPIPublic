using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecChatWebAPI.Addition;
using SecChatWebAPI.Models;
using SecChatWebAPI.Responses;
using System.Text.Json;
using System.Xml.Linq;

namespace SecChatWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IWebHostEnvironment appEnvironment;

        public ChatController(IWebHostEnvironment appEnvironment)
        {
            this.appEnvironment = appEnvironment;
        } 

        [HttpPost(Name = "CreateDialog")]
        [Authorize]
        public string CreateDialog([FromForm] string firstUser,[FromForm] string secondUser)
        {
            using (var db = new ApplicationContext())
            {
                var answer = new DialogAnswer();
                if (DbHelper.DialogExist(firstUser, secondUser))
                {
                    answer.answer = false;
                    return JsonSerializer.Serialize(answer);
                }
                var chat = db.Chats.Add(new Chat()
                {
                    isDialog = true,
                    ChatDateCreated = DateTime.UtcNow,
                    ChatUpdateTime = DateTime.UtcNow
                });
                db.SaveChanges();
                int chatid;
                chat.CurrentValues.TryGetValue("ChatId", out chatid);
                db.UsersChats.Add(new UserChat()
                {
                    ChatId = chatid,
                    UserId = firstUser
                });
                if (firstUser != secondUser)
                {
                    db.UsersChats.Add(new UserChat()
                    {
                        ChatId = chatid,
                        UserId = secondUser
                    });
                }
                db.SaveChanges();
                answer.answer = true;
                return JsonSerializer.Serialize(answer);
            }
        }

        [HttpPost(Name = "BlockChat")]
        [Authorize]
        public string BlockChat([FromForm] string userId, [FromForm] int chatId)
        {
            using (var db = new ApplicationContext())
            {
                db.UsersChats.FirstOrDefault(x => x.UserId == userId && x.ChatId == chatId).ChatIsBlocked = true;
                db.SaveChanges();
                var answer = new DialogAnswer();
                answer.answer = true;
                return JsonSerializer.Serialize(answer);
            }
        }

        [HttpPost(Name = "UnBlockChat")]
        [Authorize]
        public string UnBlockChat([FromForm] string userId, [FromForm] int chatId)
        {
            using (var db = new ApplicationContext())
            {
                db.UsersChats.FirstOrDefault(x => x.UserId == userId && x.ChatId == chatId).ChatIsBlocked = false;
                db.SaveChanges();
                var answer = new DialogAnswer();
                answer.answer = true;
                return JsonSerializer.Serialize(answer);
            }
        }

        [HttpPost(Name = "GetBlockChats")]
        [Authorize]
        public async Task<IEnumerable<Chat>> GetBlockChats([FromForm] string userId)
        {
            using (var db = new ApplicationContext())
            {
                var list = (await db.UsersChats.ToListAsync()).Where(x => x.UserId == userId && x.ChatIsBlocked == true).Select(x => db.Chats.First(c=>c.ChatId == x.ChatId)).ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    var chat = list[i];
                    if (chat.isDialog == true)
                    {
                        var altUser = DbHelper.GetAltUser(chat, userId);
                        chat.ChatName = altUser.UserName;
                        chat.ChatImg = altUser.UserImg;
                    }
                }
                return list.OrderBy(x => x.ChatUpdateTime).Reverse();
            }
        }

        [HttpPost(Name = "AddFile")]
        public async Task<DialogAnswer> AddFile([FromForm] IFormFile uploadedFile)
        {         
            if (uploadedFile != null)
            {
                var mas = uploadedFile.FileName.Split('.');
                var extension = mas[mas.Length-1];
                if (extension != "png" && extension != "jpg")
                {
                    return null;
                }

                string path = "/Files/" + uploadedFile.FileName;
                using (var fileStream = new FileStream(appEnvironment.ContentRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                FileInfo fileInfo = new FileInfo(path);
                using (var context = new ApplicationContext())
                {
                    var newPath = fileInfo.FullName.Replace(@"\", @"/");
                    var messagefile = context.MessageFiles.Add(new MessageFile()
                    {
                        Name = uploadedFile.FileName,
                        Path = path,
                        SendTime = DateTime.UtcNow
                    });
                    context.SaveChanges();                    
                    int id;
                    messagefile.CurrentValues.TryGetValue("FileId", out id);
                    return new DialogAnswer() { addition = id };
                }
            }
            return null;
        }
    }
}
