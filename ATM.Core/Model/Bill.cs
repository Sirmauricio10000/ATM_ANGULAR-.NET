using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATM.Core.Model
{
    public class Bill
    {
        public Bill()
        {
        }

        public Bill(int denomination, int quantity)
        {
            Denomination = denomination;
            Quantity = quantity;
        }

        [Key]
        public int Denomination { get; set; }

        public int Quantity { get; set; }

        [NotMapped]
        public int Ammount => Denomination * Quantity;
    }
}