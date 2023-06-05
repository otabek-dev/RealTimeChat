using Microsoft.EntityFrameworkCore;

using RealTimeMail.Model;

namespace RealTimeMail
{
    public class AppDbContext : DbContext
    {
        public DbSet<Message> ChatMessages { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();   // удаляем бд со старой схемой
            //Database.EnsureCreated();   // создаем бд с новой схемой
        }
    }
}
