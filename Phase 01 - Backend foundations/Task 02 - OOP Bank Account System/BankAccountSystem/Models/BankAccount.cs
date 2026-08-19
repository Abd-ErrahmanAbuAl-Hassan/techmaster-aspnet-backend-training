namespace Task_02_OOP_Bank_Account_System.BankAccountSystem.Models
{
    public class BankAccount
    {
        public string AccountNumber { get; }
        public Customer Customer { get; }
        public AccountType AccountType { get; }
        public DateTime CreatedAt { get; }
        public bool IsActive { get; private set; }

        private decimal _balance;
        public decimal Balance => _balance;


        private readonly List<Transaction> _transactions;
        public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

        public BankAccount(Customer customer, decimal initialBalance, AccountType accountType)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (initialBalance < 0)
                throw new ArgumentException("Initial balance cannot be negative.", nameof(initialBalance));

            AccountNumber = GenerateAccountNumber();
            Customer = customer;
            AccountType = accountType;
            _balance = initialBalance;
            CreatedAt = DateTime.Now;
            IsActive = true;
            _transactions = new List<Transaction>();

            // Record initial deposit if balance > 0
            if (initialBalance > 0)
            {
                var initialTransaction = new Transaction(
                    AccountNumber,
                    TransactionType.Deposit,
                    initialBalance,
                    _balance,
                    "Account created with initial deposit");
                _transactions.Add(initialTransaction);
            }
        }

        public Transaction Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));

            if (!IsActive)
                throw new InvalidOperationException("Cannot deposit to an inactive account.");

            _balance += amount;

            var transaction = new Transaction(
                AccountNumber,
                TransactionType.Deposit,
                amount,
                _balance,
                $"Deposit of {amount:C}");

            _transactions.Add(transaction);
            return transaction;
        }

        public Transaction Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));

            if (amount > _balance)
                throw new InvalidOperationException(
                    $"Insufficient balance. Current balance: {_balance:C}, Requested withdrawal: {amount:C}");

            if (!IsActive)
                throw new InvalidOperationException("Cannot withdraw from an inactive account.");

            _balance -= amount;

            var transaction = new Transaction(
                AccountNumber,
                TransactionType.Withdraw,
                amount,
                _balance,
                $"Withdrawal of {amount:C}");

            _transactions.Add(transaction);
            return transaction;
        }

        public Transaction RecordTransferOut(decimal amount, string destinationAccountNumber)
        {
            if (amount <= 0)
                throw new ArgumentException("Transfer amount must be greater than zero.", nameof(amount));

            if (amount > _balance)
                throw new InvalidOperationException(
                    $"Insufficient balance for transfer. Current balance: {_balance:C}, Requested transfer: {amount:C}");

            if (!IsActive)
                throw new InvalidOperationException("Cannot transfer from an inactive account.");

            _balance -= amount;

            var transaction = new Transaction(
                AccountNumber,
                TransactionType.TransferOut,
                amount,
                _balance,
                $"Transfer to account {destinationAccountNumber}");

            _transactions.Add(transaction);
            return transaction;
        }

        public Transaction RecordTransferIn(decimal amount, string sourceAccountNumber)
        {
            if (amount <= 0)
                throw new ArgumentException("Transfer amount must be greater than zero.", nameof(amount));

            if (!IsActive)
                throw new InvalidOperationException("Cannot transfer to an inactive account.");

            _balance += amount;

            var transaction = new Transaction(
                AccountNumber,
                TransactionType.TransferIn,
                amount,
                _balance,
                $"Transfer from account {sourceAccountNumber}");

            _transactions.Add(transaction);
            return transaction;
        }

        // Generates a unique account number in format ACC-XXX.
        private static string GenerateAccountNumber()
        {
            var random = new Random();
            var randomPart = random.Next(100, 999);
            return $"ACC-{randomPart}";
        }
    }
}