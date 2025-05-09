
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


            // WishlistItem relationships
            builder.Entity<WishlistItem>()
                .HasOne(wi => wi.Wishlist)
                .WithMany(w => w.WishlistItems)
                .HasForeignKey(wi => wi.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WishlistItem>()
                .HasOne(wi => wi.Book)
                .WithMany()
                .HasForeignKey(wi => wi.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // Wishlist relationships
            builder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-one relationship between User and Cart
            builder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship between Cart and CartItems
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship between CartItem and Book
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Book)
                .WithMany()
                .HasForeignKey(ci => ci.BookId)
                .OnDelete(DeleteBehavior.Cascade);


        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
         public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }


    }
}
