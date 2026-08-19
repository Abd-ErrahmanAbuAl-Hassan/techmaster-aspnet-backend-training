using Task_02_OOP_Bank_Account_System.BankAccountSystem.Services;
using Task_02_OOP_Bank_Account_System.BankAccountSystem.UI;

namespace Task_02_OOP_Bank_Account_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bankService = new BankService();

            var consoleMenu = new ConsoleMenu(bankService);

            consoleMenu.Run();
        }
    }
}
