using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecChatWebAPI.Models
{
    [Table("MessageFiles", Schema = "Main")]
    [PrimaryKey("FileId")]
    public class MessageFile
    {
        public int FileId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }     
        public DateTime SendTime { get; set; }
    }
}
