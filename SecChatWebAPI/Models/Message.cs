using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecChatWebAPI.Models
{
    [Table("Messages", Schema = "Main")]
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        public string MessageUserId { get; set; }
        public int MessageChatId { get; set; }
        public bool MessageIsRead { get; set; }
        public DateTime? MessageSendTime { get; set; }
        public int? MessageAdditionsId { get; set; }
    }
}
