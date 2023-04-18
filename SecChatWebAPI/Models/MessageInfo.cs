namespace SecChatWebAPI.Models
{
    public class MessageInfo
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int? MessageId { get; set; }
        public string? Message { get; set; }
        public string? UserImg { get; set; }
        public int? ChatId { get; set; }

        public string? TimeSend { get; set; }
        public string? pathsAddition { get; set; }
    }
}
