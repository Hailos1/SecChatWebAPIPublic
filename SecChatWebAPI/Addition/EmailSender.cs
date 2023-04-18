using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
namespace SecChatWebAPI.Models
{
    public class EmailSender
    {
        private SmtpClient smtpClient;
        //На этот адрес писать бесполезно
        private readonly MailAddress from = new MailAddress("kabanovworkprogram@gmail.com", "SecurityChat");
        public EmailSender()
        {
            smtpClient = new SmtpClient("smtp.gmail.com", 587);
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string emailPassword = config.GetSection("Passwords")["emailPassword"];
            smtpClient.Credentials = new NetworkCredential("kabanovworkprogram@gmail.com", emailPassword);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
        }

        async public Task<bool> SendCode(string emailTo, string Text)
        {
            try
            {
                MailAddress to = new MailAddress(emailTo);
                MailMessage message = new MailMessage(from, to);
                message.Subject = "Код подтверждения";
                message.Body = Text;
                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        async public Task<bool> SendMessage(string emailTo, string Text, string subject)
        {
            try
            {
                MailAddress to = new MailAddress(emailTo);
                MailMessage message = new MailMessage(from, to);
                message.Subject = subject;
                message.Body = Text;
                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
