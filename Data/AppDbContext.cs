using ThomasianMemoir.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ThomasianMemoir.Data
{
    public class AppDbContext : IdentityDbContext<User>//: DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<UserInfo> UserInfo { get; set; }
        public DbSet<UserPost> UserPost { get; set; }
        public DbSet<UserPostLikes> UserPostLikes { get; set; }
        public DbSet<UserPostComments> UserPostComments { get; set; }
        public DbSet<UserPostMedia> UserPostMedia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1-1 relationship of User to UserInfo
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserInfo)
                .WithOne(up => up.User)
                .HasForeignKey<UserInfo>(up => up.UserId)
                .HasPrincipalKey<User>(u => u.Id);

            // 1-Many relationship of UserInfo to UserPost
            modelBuilder.Entity<UserInfo>()
                .HasMany(ui => ui.Posts)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId);

            // 1-Many relationship of USerPost to UserPostLikes, UserPostComments, UerPostMedia
            modelBuilder.Entity<UserPost>()
                .HasMany(up => up.Media)
                .WithOne(upm => upm.Post)
                .HasForeignKey(upm => upm.PostId);

            modelBuilder.Entity<UserPost>()
                .HasMany(up => up.Likes)
                .WithOne(upl => upl.Post)
                .HasForeignKey(upl => upl.PostId);

            modelBuilder.Entity<UserPost>()
                .HasMany(up => up.Comments)
                .WithOne(upc => upc.Post)
                .HasForeignKey(upc => upc.PostId);

            /*modelBuilder.Entity<Users>().HasData(
                new Users()
                {
                    
                }
            );*/
        }
    }
}
