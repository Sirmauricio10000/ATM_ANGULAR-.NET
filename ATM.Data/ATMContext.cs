using ATM.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace ATM.Data
{
    public class ATMContext : DbContext
    {
        public ATMContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Bill> Bills { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<AmountTransaction> AmountTransactions { get; set; }
    }
}