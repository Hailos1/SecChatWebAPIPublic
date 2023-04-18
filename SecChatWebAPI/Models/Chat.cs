using System.ComponentModel.DataAnnotations.Schema;

namespace SecChatWebAPI.Models
{
    [Table("Chats", Schema = "Main")]
    public class Chat
    {
        public int ChatId { get; set; }
        public string? ChatName { get; set; }
        public string? ChatImg { get; set; }
        public bool isDialog { get; set; }
        public DateTime? ChatDateCreated { get; set; }
        public DateTime? ChatUpdateTime { get; set; }
    }
}
