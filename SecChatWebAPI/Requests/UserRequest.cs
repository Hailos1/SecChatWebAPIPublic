namespace SecChatWebAPI.Requests
{
    public class UserRequest
    {
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? UserName { get; set; }
        public IFormFile? formFile { get; set; }
    }
}
