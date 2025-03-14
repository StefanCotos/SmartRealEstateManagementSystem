using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Estate> Estates { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Favorite> Favorites { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<Review> ReviewUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            const string Smallint = "smallint";
            const string UuidGenerateV4 = "uuid_generate_v4()";
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Estate>(entity =>
            {
                entity.ToTable("Estates");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql(UuidGenerateV4)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql(UuidGenerateV4)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Bedrooms)
                    .IsRequired()
                    .HasColumnType(Smallint);

                entity.Property(e => e.Bathrooms)
                    .IsRequired()
                    .HasColumnType(Smallint);

                entity.Property(e => e.LandSize)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ZipCode)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.HouseSize)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ListingData)
                    .IsRequired();

                entity.Property(e => e.PriceId)
                    .HasMaxLength(50)
                    .HasDefaultValue(null);

                entity.Property(e => e.ProductId)
                    .HasMaxLength(50)
                    .HasDefaultValue(null);

                entity.Property(e => e.IsSold)
                    .HasColumnType("boolean")
                    .HasDefaultValue(false);

                entity.Property(e => e.BuyerId)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql(UuidGenerateV4)
                    .HasDefaultValue(null);

                modelBuilder.Entity<Estate>()
                    .HasOne(e => e.User)
                    .WithMany(u => u.Estates)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ModelBuilder for Entity<User>

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql(UuidGenerateV4)
                    .ValueGeneratedOnAdd();

                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.IsAdmin)
                   .HasColumnType("boolean")
                   .HasDefaultValue(false);

                entity.Property(e => e.IsEmailConfirmed)
                   .HasColumnType("boolean")
                   .HasDefaultValue(false);

                entity.Property(e => e.EmailConfirmationToken)
                     .HasMaxLength(500)
                     .HasDefaultValue(null);

                entity.Property(e => e.EmailConfirmationTokenExpires)
                     .HasDefaultValue(null);
            });

            // ModelBuilder for Entity<Contact>
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contacts");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Phone)
                    .IsRequired()
                    .HasMaxLength(20);


                entity.HasOne<User>()
                    .WithOne()
                    .HasForeignKey<Contact>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ModelBuilder for Entity<Favorite>
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.ToTable("Favorites");

                entity.HasKey(f => new { f.UserId, f.EstateId });

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Estate>()
                    .WithMany()
                    .HasForeignKey(f => f.EstateId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // ModelBuilder for Entity<Image>
            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Images");

                entity.HasKey(i => i.Id);

                entity.Property(i => i.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(i => i.EstateId)
                    .IsRequired();

                entity.Property(i => i.Extension)
                    .IsRequired()
                    .HasMaxLength(10);


                entity.HasOne<Estate>()
                    .WithMany()
                    .HasForeignKey(i => i.EstateId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ModelBuilder for Entity<Report>
            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Reports");

                entity.HasKey(r => r.Id);

                entity.Property(r => r.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(r => r.BuyerId)
                    .IsRequired();

                entity.Property(r => r.SellerId)
                    .IsRequired();

                entity.Property(r => r.Description)
                    .HasMaxLength(1000);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(r => r.BuyerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(r => r.SellerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            // ModelBuilder for Entity<ReviewUser>
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("ReviewUsers");

                entity.HasKey(ru => ru.Id);

                entity.Property(ru => ru.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(ru => ru.BuyerId)
                    .IsRequired();

                entity.Property(ru => ru.SellerId)
                    .IsRequired();

                entity.Property(ru => ru.Description)
                    .HasMaxLength(1000);

                entity.Property(ru => ru.Rating)
                    .IsRequired()
                    .HasColumnType(Smallint);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(ru => ru.BuyerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(ru => ru.SellerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
        }
    }
}
