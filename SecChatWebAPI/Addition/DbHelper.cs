using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SecChatWebAPI.Models;
using System.Security.Cryptography;

namespace SecChatWebAPI.Addition
{
    public static class DbHelper
    {
        public static List<User> GetUsersBySubStringId(string subUserId)
        {
            using(var db = new ApplicationContext())
            {
                var users = db.Users.AsEnumerable().Where(u => u.UserId.ToLower().StartsWith(subUserId)).Take(15).ToList();
                return users;
            }
        }

        public static async Task<User> GetUserByEmail(string email)
        {
            using (var db = new ApplicationContext())
            {
                return await db.Users.FirstOrDefaultAsync(x => x.UserEmail == email);                
            }
        }

        public static List<Chat> GetChatsUser(string UserId)
        {
            List<int> chatsIds;
            List<Chat> chats = new List<Chat>();
            var db = new ApplicationContext();
            chatsIds = db.UsersChats.AsEnumerable().Where(u => u.UserId == UserId && u.ChatIsBlocked != true).Select(c => c.ChatId).ToList();
            var db1 = new ApplicationContext();
            foreach(var id in chatsIds)
            {
                chats.Add(db1.Chats.AsEnumerable().Where(c => c.ChatId == id).First());
            }
            return chats;
        }

        public static User GetUser(string UserId)
        {
            var db = new ApplicationContext();
            User? user = db.Users.AsEnumerable().Where(u => u.UserId == UserId).FirstOrDefault();
            return user;
        }

        public static User GetAltUser(Chat chat, string FirstUserId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var altUserId = db.UsersChats.AsEnumerable().Where(c => c.ChatId == chat.ChatId && c.UserId != FirstUserId).First().UserId;
                User altUser = db.Users.AsEnumerable().Where(u => u.UserId == altUserId).First();
                return altUser;
            }            
        }

        public static bool DialogExist(string firstUser, string secondUser)
        {
            using(var db = new ApplicationContext())
            {
                var chats = db.Chats.AsEnumerable();
                var chatsFirstUser = db.UsersChats.AsEnumerable().Where(uc => uc.UserId == firstUser).ToList();
                var chatsSecondUser = db.UsersChats.AsEnumerable().Where(uc => uc.UserId == secondUser).ToList();
                for (int i = 0; i < chatsFirstUser.Count(); i++)
                {
                    for (int j = 0; j < chatsSecondUser.Count(); j++)
                    {
                        if (chatsFirstUser[i].ChatId == chatsSecondUser[j].ChatId)
                        {
                            if (chats.Where(c => c.ChatId == chatsFirstUser[i].ChatId).First().isDialog == true)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        public static User GetAltUser(int chatid, string FirstUserId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var altUserChat = db.UsersChats.AsEnumerable().Where(c => c.ChatId == chatid && c.UserId != FirstUserId).FirstOrDefault();
                string? altUserId;
                if (altUserChat != null)
                {
                    altUserId = altUserChat.UserId;
                }
                else
                {
                    altUserId = FirstUserId;
                }
                User altUser = db.Users.AsEnumerable().Where(u => u.UserId == altUserId).First();
                return altUser;
            }
        }

        public static Message SendMessage(string userid, string message, int chatid, int fileid)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                int addId = 0;
                int? additionId = null;
                if (fileid != 0)
                {
                    var messageAddition = db.MessageAdditions.Add(new MessageAddition()
                    {
                        MessageFileId = fileid
                    });
                    db.SaveChanges();
                    messageAddition.CurrentValues.TryGetValue("AdditionId", out addId);
                }

                if (addId != 0)
                    additionId = addId;               

                
                var sendedmessage = db.Messages.Add(new Message()
                {
                    MessageChatId = chatid,
                    MessageIsRead = true,
                    MessageSendTime = DateTime.UtcNow,
                    MessageText = TryHashCode.EncryptSha256(message),
                    MessageUserId = userid,
                    MessageAdditionsId = additionId
                });
                db.SaveChanges();

                db.Chats.FirstOrDefault(x => x.ChatId == chatid).ChatUpdateTime = DateTime.UtcNow;
                db.SaveChanges();

                int messageId;
                sendedmessage.CurrentValues.TryGetValue("MessageId", out messageId);
                return sendedmessage.Entity;
            }
        }

        public static string GetMessageFile(int? AdditionsId)
        {
            if(AdditionsId == null)
            {
                return null;
            }
            using (ApplicationContext db = new ApplicationContext())
            {
                var fileid = db.MessageAdditions.AsEnumerable().Where(a => a.AdditionId == AdditionsId).FirstOrDefault().MessageFileId;
                var path = db.MessageFiles.AsEnumerable().Where(f => f.FileId == fileid).FirstOrDefault().Path;
                if (path.Contains("/Files/"))
                {
                    path = "http://securitychat.ru" + path;
                }
                return path;
            }
        }

        public static List<Message> GetMessages(int chatid, int start, int end)
        {
            var range = new Range(start, end);
            using (ApplicationContext db = new ApplicationContext())
            {
                var messages = db.Messages.AsEnumerable();
                var messages1 = messages
                    .Where(c => c.MessageChatId == chatid)
                    .Reverse().Take(range)
                    .OrderBy(c => c.MessageSendTime)
                    .ToList();
                return messages1;
            }
        }        
    }
}
