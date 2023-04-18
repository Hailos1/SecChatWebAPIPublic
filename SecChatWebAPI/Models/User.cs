using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecChatWebAPI.Models
{
    [Table("Users", Schema = "Main")]
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string? UserDescription { get; set; }
        public string? UserImg { get; set; }
        public DateTime? UserCreated { get; set; }
        public string HashPassword { get; set; }
    }
}
