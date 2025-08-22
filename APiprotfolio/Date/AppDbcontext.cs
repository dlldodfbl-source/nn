using Microsoft.EntityFrameworkCore;
using PROtofile.Date.Models;

namespace PROtofile.Date
{
    public class AppDbcontext
    {

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Message> Messages => Set<Message>();
        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Message>().HasIndex(x => x.CreatedAt);
            b.Entity<Message>().HasIndex(x => x.Email);
        }
    }
}
}
