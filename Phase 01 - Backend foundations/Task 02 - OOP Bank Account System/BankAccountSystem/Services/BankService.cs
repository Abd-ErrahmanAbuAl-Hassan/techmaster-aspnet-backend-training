using Task_02_OOP_Bank_Account_System.BankAccountSystem.Models;

namespace Task_02_OOP_Bank_Account_System.BankAccountSystem.Services
{

    public class BankService
    {
        private readonly List<BankAccount> _accounts;

        public BankService()
        {
            _accounts = new List<BankAccount>();
        }

        public BankAccount CreateAccount(string fullName,string email,string phoneNumber,decimal initialBalance,AccountType accountType)
        {
            var customer = new Customer(fullName, email, phoneNumber);
            var account = new BankAccount(customer, initialBalance, accountType);
            _accounts.Add(account);

            return account;
        }

        public BankAccount FindAccount(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                return null;

            return _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        }

        public Transaction Deposit(string accountNumber, decimal amount)
        {
            var account = FindAccount(accountNumber);
            if (account == null)
                throw new InvalidOperationException($"Account {accountNumber} was not found.");

            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));

            return account.Deposit(amount);
        }

        public Transaction Withdraw(string accountNumber, decimal amount)
        {
            var account = FindAccount(accountNumber);
            if (account == null)
                throw new InvalidOperationException($"Account {accountNumber} was not found.");

            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));

            return account.Withdraw(amount);
        }

        public void Transfer(string sourceAccountNumber, string destinationAccountNumber, decimal amount)
        {

            var sourceAccount = FindAccount(sourceAccountNumber);
            if (sourceAccount == null)
                throw new InvalidOperationException($"Source account {sourceAccountNumber} was not found.");

            var destinationAccount = FindAccount(destinationAccountNumber);
            if (destinationAccount == null)
                throw new InvalidOperationException($"Destination account {destinationAccountNumber} was not found.");

            if (amount <= 0)
                throw new ArgumentException("Transfer amount must be greater than zero.", nameof(amount));

            if (sourceAccountNumber == destinationAccountNumber)
                throw new InvalidOperationException("Source and destination accounts must be different.");

            if (amount > sourceAccount.Balance)
                throw new InvalidOperationException(
                    $"Insufficient balance. Current balance: {sourceAccount.Balance:C}, Requested transfer: {amount:C}");

            try
            {
                sourceAccount.RecordTransferOut(amount, destinationAccountNumber);
                destinationAccount.RecordTransferIn(amount, sourceAccountNumber);
            }
            catch
            {
                throw;
            }
        }

        public IReadOnlyList<BankAccount> GetAllAccounts()
        {
            return _accounts.AsReadOnly();
        }

        public IReadOnlyList<Transaction> GetTransactionHistory(string accountNumber)
        {
            var account = FindAccount(accountNumber);
            if (account == null)
                throw new InvalidOperationException($"Account {accountNumber} was not found.");

            return account.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .ToList()
                .AsReadOnly();
        }
    }
}