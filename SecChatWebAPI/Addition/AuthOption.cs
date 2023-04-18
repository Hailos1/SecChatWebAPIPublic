using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SecChatWebAPI.Addition
{
    public class AuthOptions
    {
        public const string ISSUER = "SecurityChatServer"; // издатель токена
        public const string AUDIENCE = "SecurityChatClient"; // потребитель токена
        const string KEY = "*";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
