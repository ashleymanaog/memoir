using ThomasianMemoir.Models;
using Microsoft.EntityFrameworkCore;

namespace ThomasianMemoir.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserPost> UserPost { get; set; }
        public DbSet<UserPostLikes> UserPostLikes { get; set; }
        public DbSet<UserPostComments> UserPostComments { get; set; }
        public DbSet<UserPostMedia> UserPostMedia { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*modelBuilder.Entity<Users>().HasData(
                new Users()
                {
                    
                }
            );*/
        }
    }
}
