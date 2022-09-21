using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATM.Core.Model
{
    public class AmountTransaction
    {
        public int Denomination { get; set; }
        public int Quantity { get; set; }
        public string TransactionId { get; set; }

        [ForeignKey(nameof(TransactionId))]
        public Transaction Transaction { get; set; }

        [ForeignKey(nameof(Denomination))]
        public Bill Bill { get; set; }
    }
}