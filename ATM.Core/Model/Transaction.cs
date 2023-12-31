﻿namespace ATM.Core.Model
{
    public class Transaction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime DateTime { get; set; }

        public TransactionType Type { get; set; }


        public ICollection<AmountTransaction> Amount { get; set; } = new List<AmountTransaction>();

        public Transaction()
        {

        }
    }
}