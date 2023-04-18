using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace SecChatWebAPI.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<Chat> Chats { get; set; } = null!;
        public DbSet<UserChat> UsersChats { get; set; } = null!;
        public DbSet<Friends> Friends { get; set; } = null!;
        public DbSet<MessageAddition> MessageAdditions { get; set; } = null!;
        public DbSet<MessageFile> MessageFiles { get; set; } = null!;

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var uri = new Uri("postgres://qlovozbd:fHU6NoR-bxfmULllnluU1s7vL8APHsxP@mouse.db.elephantsql.com/qlovozbd");
            //optionsBuilder.UseSqlServer("Server=DESKTOP-FLFB621\\MSSQLSERVER01;Database=Messenger;Trusted_Connection=True;TrustServerCertificate=True;");
            //optionsBuilder.UseNpgsql($"Host={uri.Host};Port=5432;Database={uri.AbsolutePath.Trim('/')};Username={uri.UserRequest.Split(':')[0]};UserPassword=fHU6NoR-bxfmULllnluU1s7vL8APHsxP");
            //optionsBuilder.UseNpgsql($"Host=localhost;Port=5432;Database=securitychat;Username=postgres;UserPassword=563596");
            //optionsBuilder.UseNpgsql($"Host=localhost;Port=5432;Database=SecurityChat;Username=postgres;UserPassword=563596");
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            string connectionString = config.GetConnectionString("connection");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
