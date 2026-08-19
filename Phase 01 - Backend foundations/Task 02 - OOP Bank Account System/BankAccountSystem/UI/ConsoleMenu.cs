using System.Reflection.Metadata.Ecma335;
using Task_02_OOP_Bank_Account_System.BankAccountSystem.Models;
using Task_02_OOP_Bank_Account_System.BankAccountSystem.Services;
using Task_02_OOP_Bank_Account_System.BankAccountSystem.Validations;

namespace Task_02_OOP_Bank_Account_System.BankAccountSystem.UI
{
    public class ConsoleMenu
    {
        private readonly BankService _bankService;
        private bool _isRunning;

        public ConsoleMenu(BankService bankService)
        {
            _bankService = bankService ?? throw new ArgumentNullException(nameof(bankService));
            _isRunning = true;
        }
        public void Run()
        {
            DisplayWelcome();

            while (_isRunning)
            {
                DisplayMainMenu();
                HandleMenuChoice();
            }

            DisplayGoodbye();
        }

        private void DisplayWelcome()
        {
            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("             BANK SYSTEM                ");
            Console.WriteLine("========================================");
            Console.WriteLine();
        }

        private void DisplayGoodbye()
        {
            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("     Thank you for using Our Bank!      ");
            Console.WriteLine("========================================");
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("====== MAIN MENU ======");
            Console.WriteLine("1. Create Customer Account");
            Console.WriteLine("2. Deposit Money");
            Console.WriteLine("3. Withdraw Money");
            Console.WriteLine("4. Transfer Money");
            Console.WriteLine("5. View Account Details");
            Console.WriteLine("6. View Transaction History");
            Console.WriteLine("7. View All Accounts");
            Console.WriteLine("8. Exit");
            Console.WriteLine();
            Console.Write("Choose an option (1-8): ");
        }

        private void HandleMenuChoice()
        {
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                DisplayError("Invalid input. Please enter a number between 1 and 8.");
                return;
            }

            switch (choice)
            {
                case 1:
                    CreateAccount();
                    break;
                case 2:
                    DepositMoney();
                    break;
                case 3:
                    WithdrawMoney();
                    break;
                case 4:
                    TransferMoney();
                    break;
                case 5:
                    ViewAccountDetails();
                    break;
                case 6:
                    ViewTransactionHistory();
                    break;
                case 7:
                    ViewAllAccounts();
                    break;
                case 8:
                    _isRunning = false;
                    break;
                default:
                    DisplayError("Invalid option. Please choose between 1 and 8.");
                    break;
            }
        }

        private void CreateAccount()
        {
            Console.WriteLine();
            Console.WriteLine("====== CREATE NEW ACCOUNT ======");

            string fullName, email, phoneNumber;
            decimal initialBalance;
            do
            {
                fullName = GetInput("Enter full name: ");

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    DisplayError($"Name is required and cannot be empty! Try again.");
                    continue;
                }
                break;
            } while (true);

            do
            {
                email = GetInput("Enter email: ");
                var result = EmailValidator.Validate(email);
                if (!result.IsValid)
                {
                    DisplayError(result.ErrorMessage);
                    continue;
                }
                break;
            } while (true);

            do
            {
                phoneNumber = GetInput("Enter phone number: ");
                var result = PhoneValidator.Validate(phoneNumber);
                if (!result.IsValid)
                {
                    DisplayError(result.ErrorMessage);
                    continue;
                }
                phoneNumber = result.NormalizedNumber;
                break;
            } while (true);

            do
            {
                if (!decimal.TryParse(GetInput("Enter initial balance: "), out initialBalance) || initialBalance < 0)
                {
                    DisplayError("Invalid balance amount! Try again");
                    continue;
                }
                break;

            } while (true);

            var accountType = SelectAccountType();

            try
            {
                var account = _bankService.CreateAccount(
                    fullName,
                    email,
                    phoneNumber,
                    initialBalance,
                    accountType.Value);

                Console.WriteLine();
                DisplaySuccess("Account created successfully!");
                Console.WriteLine($"Account Number: {account.AccountNumber}");
                Console.WriteLine($"Account Type: {account.AccountType}");
                Console.WriteLine($"Balance: {account.Balance:C}");
                Console.WriteLine($"Customer: {account.Customer.FullName}");
            }
            catch (ArgumentException ex)
            {
                DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }

        }

        private void DepositMoney()
        {
            Console.WriteLine();
            Console.WriteLine("====== DEPOSIT MONEY ======");

            string accountNumber;
            decimal amount;
            do
            {
                accountNumber = GetInput("Enter account number: ");

                if (string.IsNullOrWhiteSpace(accountNumber))
                {
                    DisplayError("Account Number is required! Try again");
                    continue;
                }
                break;
            } while (true);

            do
            {
                if (!decimal.TryParse(GetInput("Enter deposit amount: "), out amount) || amount <= 0)
                {
                    DisplayError("Invalid amount! Try again");
                    continue;
                }
                break;
            } while (true);

            try
            {
                var transaction = _bankService.Deposit(accountNumber, amount);
                var account = _bankService.FindAccount(accountNumber);

                Console.WriteLine();
                DisplaySuccess("Deposit successful!");
                Console.WriteLine($"Amount: {transaction.Amount:C}");
                Console.WriteLine($"New Balance: {transaction.BalanceAfterTransaction:C}");
                Console.WriteLine($"Date: {transaction.TransactionDate:G}");
            }
            catch (InvalidOperationException ex)
            {
                DisplayError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void WithdrawMoney()
        {
            Console.WriteLine();
            Console.WriteLine("====== WITHDRAW MONEY ======");

            string accountNumber;
            decimal amount;
            do
            {
                accountNumber = GetInput("Enter account number: ");

                if (string.IsNullOrWhiteSpace(accountNumber))
                {
                    DisplayError("Account Number is required! Try again");
                    continue;
                }
                break;
            } while (true);

            do
            {
                if (!decimal.TryParse(GetInput("Enter deposit amount: "), out amount) || amount <= 0)
                {
                    DisplayError("Invalid amount! Try again");
                    continue;
                }
                break;
            } while (true);

            try
                {
                    var transaction = _bankService.Withdraw(accountNumber, amount);

                    Console.WriteLine();
                    DisplaySuccess("Withdrawal successful!");
                    Console.WriteLine($"Amount: {transaction.Amount:C}");
                    Console.WriteLine($"New Balance: {transaction.BalanceAfterTransaction:C}");
                    Console.WriteLine($"Date: {transaction.TransactionDate:G}");
                }
                catch (InvalidOperationException ex)
                {
                    DisplayError(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    DisplayError(ex.Message);
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }

        }

        private void TransferMoney()
        {
            Console.WriteLine();
            Console.WriteLine("====== TRANSFER MONEY ======");

            string sourceAccountNumber, destinationAccountNumber;
            decimal amount;
            do
            {
                sourceAccountNumber = GetInput("Enter source account number: ");

                if (string.IsNullOrWhiteSpace(sourceAccountNumber))
                {
                    DisplayError("Account Number is required! Try again");
                    continue;
                }
                break;
            } while (true);

            do
            {
                destinationAccountNumber = GetInput("Enter destination account number: ");

                if (string.IsNullOrWhiteSpace(sourceAccountNumber))
                {
                    DisplayError("Account Number is required! Try again");
                    continue;
                }
                break;
            } while (true);

            do
            {
                if (!decimal.TryParse(GetInput("Enter transfer amount: "), out amount) || amount <=0)
                {
                    DisplayError("Invalid amount.");
                    continue;
                }
                break;
            } while (true);

            try
            {
                _bankService.Transfer(sourceAccountNumber, destinationAccountNumber, amount);

                var sourceAccount = _bankService.FindAccount(sourceAccountNumber);
                var destinationAccount = _bankService.FindAccount(destinationAccountNumber);

                Console.WriteLine();
                DisplaySuccess("Transfer successful!");
                Console.WriteLine($"Amount Transferred: {amount:C}");
                Console.WriteLine($"Source Account New Balance: {sourceAccount.Balance:C}");
                Console.WriteLine($"Destination Account New Balance: {destinationAccount.Balance:C}");
            }
            catch (InvalidOperationException ex)
            {
                DisplayError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }

        }

        private void ViewAccountDetails()
        {
            Console.WriteLine();
            Console.WriteLine("====== ACCOUNT DETAILS ======");

            string accountNumber;
            do
            {
                accountNumber = GetInput("Enter account number: ");
                if (string.IsNullOrWhiteSpace(accountNumber))
                {
                    DisplayError("Account Number is required! Try again");
                    continue;
                }
                break;
            } while (true);

            try
            {
                var account = _bankService.FindAccount(accountNumber);
                if (account == null)
                {
                    DisplayError("Account was not found.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine("--- Account Information ---");
                Console.WriteLine($"Account Number: {account.AccountNumber}");
                Console.WriteLine($"Customer Name: {account.Customer.FullName}");
                Console.WriteLine($"Email: {account.Customer.Email}");
                Console.WriteLine($"Phone: {account.Customer.PhoneNumber}");
                Console.WriteLine($"Account Type: {account.AccountType}");
                Console.WriteLine($"Balance: {account.Balance:C}");
                Console.WriteLine($"Created Date: {account.CreatedAt:G}");
                Console.WriteLine($"Status: {(account.IsActive ? "Active" : "Inactive")}");
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }

        }

        private void ViewTransactionHistory()
        {
            Console.WriteLine();
            Console.WriteLine("====== TRANSACTION HISTORY ======");

            string accountNumber;
            do
            {
                accountNumber = GetInput("Enter account number: ");

                if (string.IsNullOrWhiteSpace(accountNumber))
                {
                    DisplayError("Account Number is required! Try again");
                    continue;
                }
                break;
            } while (true);

            try
            {
                var transactions = _bankService.GetTransactionHistory(accountNumber);

                if (transactions.Count == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("This account has no transactions.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine($"Transaction History for Account {accountNumber}:");
                const int TOTAL_WIDTH = 69;
                Console.WriteLine(new string('═', TOTAL_WIDTH));
                Console.WriteLine($"{"Type",-15} {"Amount",-15} {"Balance After",-15} {"Date",-25}");
                Console.WriteLine(new string('─', TOTAL_WIDTH));

                foreach (var transaction in transactions)
                {
                    Console.WriteLine(
                        $"{transaction.TransactionType,-15} " +
                        $"{transaction.Amount.ToString("C2"),-15} " +           
                        $"{transaction.BalanceAfterTransaction.ToString("C2"),-15} " + 
                        $"{transaction.TransactionDate.ToString("G"),-25}");    
                }

                Console.WriteLine(new string('═', TOTAL_WIDTH));
            }
            catch (InvalidOperationException ex)
            {
                DisplayError(ex.Message);
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }

        }

        private void ViewAllAccounts()
        {
            Console.WriteLine();
            Console.WriteLine("====== ALL ACCOUNTS ======");

            try
            {
                var accounts = _bankService.GetAllAccounts();

                if (accounts.Count == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("No accounts exist in the system.");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine($"Total Accounts: {accounts.Count}");
                const int TOTAL_WIDTH = 80;
                Console.WriteLine(new string('═', TOTAL_WIDTH));
                Console.WriteLine(
                    $"{"Account #",-15} " +
                    $"{"Customer",-25} " +
                    $"{"Type",-15} " +
                    $"{"Balance",-15} " +
                    $"{"Status",-10}");
                Console.WriteLine(new string('─', TOTAL_WIDTH));

                foreach (var account in accounts)
                {
                    Console.WriteLine(
                        $"{account.AccountNumber,-15} " +
                        $"{account.Customer.FullName,-25} " +
                        $"{account.AccountType,-15} " +
                        $"{account.Balance.ToString("C"),-15} " +
                        $"{(account.IsActive ? "Active" : "Inactive"),-10}");
                }

                Console.WriteLine(new string('═', TOTAL_WIDTH));
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }

        private void DisplaySuccess(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{message}");
            Console.ForegroundColor = originalColor;
        }

        private void DisplayError(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.ForegroundColor = originalColor;
        }

        private AccountType? SelectAccountType()
        {
            do
            {
                Console.WriteLine();
                Console.WriteLine("Select Account Type: ");
                Console.WriteLine("1. Checking");
                Console.WriteLine("2. Savings");
                Console.WriteLine("3. Business");
                Console.WriteLine("4. StudentSavings");
                Console.Write("Enter choice (1-4): ");

                if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > 4)
                {
                    DisplayError("Invalid account type selection! Try again.");
                    continue;
                }

                return choice switch
                {
                    1 => AccountType.Checking,
                    2 => AccountType.Savings,
                    3 => AccountType.Business,
                    4 => AccountType.StudentSavings,
                    _ => null
                };
            } while (true);
        }

        private string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

    }
}