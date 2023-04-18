using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using SecChatWebAPI.Models;
using System.Security.Claims;
using SecChatWebAPI.Addition;
using System.Net.WebSockets;
using System.Text;

namespace SecChatWebAPI.Hubs
{
    public class MessengerHub : Hub
    {
        [Authorize]
        public async Task SendMessage(string user, string message, string chatid, int fileid = 0)
        {
            if ((message == "" || message == "\n") && fileid != 0)
                return;
            var db = new ApplicationContext();
            string pathFile = null;
            var list = db.UsersChats.AsEnumerable().Where(u => u.ChatId == Convert.ToInt32(chatid)).Select(s => s.UserId);
            var userMessage = db.Users.AsEnumerable().Where(v => v.UserId == user).First();
            var messageEntity = DbHelper.SendMessage(user, message, Convert.ToInt32(chatid), fileid);
            if (fileid != 0)
                pathFile = DbHelper.GetMessageFile(db.Messages.AsEnumerable().Where(m => m.MessageId == messageEntity.MessageId).First().MessageAdditionsId.Value);
            var messageInfo = new MessageInfo()
            {
                UserId = user,
                UserImg = userMessage.UserImg,
                UserName = userMessage.UserName,
                Message = message,
                MessageId = messageEntity.MessageId,
                TimeSend = messageEntity.MessageSendTime.ToString(),
                ChatId = Convert.ToInt32(chatid),
                pathsAddition = pathFile
            };
            if (db.Chats.AsEnumerable().Where(c => c.ChatId == messageEntity.MessageChatId).First().isDialog == true)
            {
                var altUserId = db.UsersChats.AsEnumerable().Where(u => u.ChatId == Convert.ToInt32(chatid)).Where(s => s.UserId != user).FirstOrDefault().UserId;
                var altUser = db.Users.AsEnumerable().Where(u => u.UserId == altUserId).First();
                //new EmailSender().SendMessage(altUser.UserEmail, $"Вы получили сообщение от пользователя {userMessage.UserName}", "Новое сообщение");
            }
            await Clients.Users(list).SendAsync("ReceiveMessage", JsonSerializer.Serialize(messageInfo));
        }

        [Authorize]
        public async Task GetMessages(string user, string chatid, int start, int end)
        {
            var db = new ApplicationContext();
            var s = DbHelper.GetMessages(Convert.ToInt32(chatid), start, end);
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
            await Clients.User(user).SendAsync("receiveMessages", JsonSerializer.Serialize(messageInfoEnum.Reverse()));
        }

        public async Task AddToGroup(string chatid)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatid);
        }

        public async Task RemoveFromGroup(string chatid)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatid);
        }

        //public async Task OnConnectedAsync(WebSocket webSocket)
        //{
        //    var buffer = new byte[1024 * 4];
        //    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        //    while (!result.CloseStatus.HasValue)
        //    {
        //        // Process received data
        //        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
        //        Console.WriteLine($"Received message: {receivedMessage}");

        //        // Send message back to the client
        //        string responseMessage = $"Response to {receivedMessage}";
        //        byte[] responseBuffer = Encoding.UTF8.GetBytes(responseMessage);
        //        await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer, 0, responseBuffer.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

        //        // Receive next message
        //        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //    }

        //    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        //}
    }
}
