using ATM.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace ATM.Data
{
    public class ATMContext : DbContext
    {
        public ATMContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Bill>()
                .HasData(new[]
                {
                    new Bill(10_000, 30),
                    new Bill(20_000, 25),
                    new Bill(50_000, 20),
                    new Bill(100_000, 15),
                });

            builder.Entity<AmountTransaction>()
                .HasKey(x => new { x.TransactionId, x.Denomination });
        }

        public DbSet<Bill> Bills { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<AmountTransaction> AmountTransactions { get; set; }
    }
}