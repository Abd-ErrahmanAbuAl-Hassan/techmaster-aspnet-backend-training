namespace Task_02_OOP_Bank_Account_System.BankAccountSystem.Models
{
    public class Transaction
    {
        public string TransactionId { get; }
        public string AccountNumber { get; }
        public TransactionType TransactionType { get; }
        public decimal Amount { get; }
        public DateTime TransactionDate { get; }
        public string Description { get; }
        public decimal BalanceAfterTransaction { get; }

        public Transaction(
            string accountNumber,
            TransactionType transactionType,
            decimal amount,
            decimal balanceAfterTransaction,
            string description)
        {
            TransactionId = Guid.NewGuid().ToString().Substring(0, 12).ToUpper();
            AccountNumber = accountNumber;
            TransactionType = transactionType;
            Amount = amount;
            TransactionDate = DateTime.Now;
            Description = description;
            BalanceAfterTransaction = balanceAfterTransaction;
        }
    }
}