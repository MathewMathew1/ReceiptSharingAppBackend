using Microsoft.EntityFrameworkCore;

namespace ReceiptSharing.Api.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SubscriptionReceipt> SubscriptionsReceipt { get; set; }
        public DbSet<SubscriptionUser> SubscriptionsUser { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        ILogger<AppDbContext> _logger;

        private IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration, ILogger<AppDbContext> logger)
            : base(options)
        {
            _logger = logger;
            _configuration = configuration;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration["DefaultConnection"];
            _logger.LogInformation(connectionString);
            optionsBuilder.UseNpgsql(connectionString);
        }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Receipt>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Receipt>()
                .HasOne(r => r.User)
                .WithMany(u => u.Receipts)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Receipt>()
                .OwnsMany(r => r.Ingredients, a =>
                {
                    a.Property<string>("Name"); // Name will be part of the key
                    a.Property<double>("Quantity");
                    a.Property<string>("Unit");
                    a.Property<bool>("Optional");
                });
            modelBuilder.Entity<Receipt>()
            .HasIndex(r => r.CreatedAt)
            .IsUnique(false);
            
            modelBuilder.Entity<Rating>()
                .HasKey(r => new { r.UserId, r.ReceiptId });

            modelBuilder.Entity<Review>()
                .HasKey(r => new { r.UserId, r.ReceiptId });

            modelBuilder.Entity<SubscriptionReceipt>()
                .HasKey(s => new { s.UserId, s.ReceiptId });

            modelBuilder.Entity<SubscriptionReceipt>()
                .HasOne(s => s.User)
                .WithMany(u => u.SubscriptionsReceipt)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubscriptionReceipt>()
                .HasOne(s => s.Receipt)
                .WithMany(r => r.SubscriptionsReceipt)
                .HasForeignKey(s => s.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubscriptionUser>()
                .HasKey(s => new { s.UserId, s.UserSubscribedToId });

            modelBuilder.Entity<SubscriptionUser>()
                .HasOne(s => s.User)
                .WithMany(r => r.Subscribers)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubscriptionUser>()
                .HasOne(s => s.UserSubscribedTo)
                .WithMany(u => u.SubscribedTo)
                .HasForeignKey(s => s.UserSubscribedToId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Receipt)
                .WithMany(r => r.Ratings)
                .HasForeignKey(r => r.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Receipt)
                .WithMany(r => r.Reviews)
                .HasForeignKey(r => r.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);
            
            

        }
    }
}