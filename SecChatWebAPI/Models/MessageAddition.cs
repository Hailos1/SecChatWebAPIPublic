using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecChatWebAPI.Models
{
    [Table("MessageAdditions", Schema = "Main")]
    [PrimaryKey("AdditionId")]
    public class MessageAddition
    {                       
        public int AdditionId { get; set; }
        public int? MessageId { get; set; }
        public int MessageFileId { get; set; }
    }
}
