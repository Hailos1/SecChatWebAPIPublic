using System.ComponentModel.DataAnnotations.Schema;

namespace SecChatWebAPI.Models
{
    [Table("Friends", Schema = "Main")]
    public class Friends
    {
        public int FriendsId { get; set; }

        public string FirstUserId { get; set; }

        public string SecondUserId { get; set; }
    }
}
