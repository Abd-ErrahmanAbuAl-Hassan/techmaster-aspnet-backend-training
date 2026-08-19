using Task_03___Employee_Management_Console_App.Services;
using Task_03___Employee_Management_Console_App.UI;

namespace Task_03___Employee_Management_Console_App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var employeeService = new EmployeeService();
            var reportService = new EmployeeReportService(employeeService);

            var menu = new ConsoleMenu(employeeService, reportService);
            menu.Run();
        }
    }
}
