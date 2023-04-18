using System.ComponentModel.DataAnnotations.Schema;

namespace SecChatWebAPI.Models
{
    [Table("UsersChats", Schema = "Main")]
    public class UserChat
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int ChatId { get; set; }
        public bool ChatIsBlocked { get; set; }
    }
}
