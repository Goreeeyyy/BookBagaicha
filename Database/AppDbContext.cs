using System.Reflection.Emit;
using BookBagaicha.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookBagaicha.Database
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole<long>>().HasData(

                new IdentityRole<long>
                {
                    Id = 1,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "505beae6-0842-470c-8228-83709f52221c"
                },

                new IdentityRole<long>
                {
                    Id = 2,
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = "7d144b5e-5c7b-47bf-8e64-bfc631bd0984"
                },

                new IdentityRole<long>
                {
                    Id = 3,
                    Name = "Staff",
                    NormalizedName = "STAFF",
                    ConcurrencyStamp = "e4dbac8e-49d6-499a-b491-628dad6c5742"
                }


                );
           builder.Entity<Book>()
     .HasMany(b => b.Authors)
     .WithMany(a => a.Books)
     .UsingEntity(j => j.ToTable("BookAuthors"));

        builder.Entity<Book>()
         .HasOne(b => b.Publisher)
         .WithMany(p => p.Books)
         .HasForeignKey(b => b.PublisherId);


        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

    }
}
