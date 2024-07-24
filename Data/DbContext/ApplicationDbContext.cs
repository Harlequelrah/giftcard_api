using Microsoft.EntityFrameworkCore;
using giftcard_api.Models;
namespace giftcard_api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<SubscriberWallet> SubscriberWallets { get; set; }
        public DbSet<SubscriberHistory> SubscriberHistories { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<BeneficiaryWallet> BeneficiaryWallets { get; set; }
        public DbSet<MerchantWallet> MerchantWallets { get; set; }
        public DbSet<MerchantHistory> MerchantHistories { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<BeneficiaryHistory> BeneficiaryHistories { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SubscriberHistory>()
                .Property(a => a.Action)
                .HasConversion(
                    b => b.ToString(),
                    b => (SubscriberHistory.SubscriberActions)Enum.Parse(typeof(SubscriberHistory.SubscriberActions), b));

            modelBuilder.Entity<MerchantHistory>()
                .Property(a => a.Action)
                .HasConversion(
                    b => b.ToString(),
                    b => (MerchantHistory.MerchantActions)Enum.Parse(typeof(MerchantHistory.MerchantActions), b));

            modelBuilder.Entity<BeneficiaryHistory>()
                .Property(a => a.Action)
                .HasConversion(
                    b => b.ToString(),
                    b => (BeneficiaryHistory.BeneficiaryActions)Enum.Parse(typeof(BeneficiaryHistory.BeneficiaryActions), b));

            modelBuilder.Entity<Subscription>()
                .HasKey(pk => new { pk.IdSubscriber, pk.IdPackage });


            modelBuilder.Entity<Subscription>()
                .HasOne(pa => pa.Subscriber)
                .WithMany(p => p.Packages)
                .HasForeignKey(pa => pa.IdSubscriber);

            modelBuilder.Entity<Subscription>()
                .HasOne(pa => pa.Package)
                .WithMany(a => a.Subscribers)
                .HasForeignKey(pa => pa.IdPackage);
        }

    }
}
