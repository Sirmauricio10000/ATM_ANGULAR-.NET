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
                    new Bill(10, 30),
                    new Bill(20, 25),
                    new Bill(50, 20),
                    new Bill(100, 15),
                });

            builder.Entity<AmountTransaction>()
                .HasKey(x => new { x.TransactionId, x.Denomination });
        }

        public DbSet<Bill> Bills { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<AmountTransaction> AmountTransactions { get; set; }
    }
}