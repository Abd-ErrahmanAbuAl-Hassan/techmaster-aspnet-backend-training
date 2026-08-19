using Task_03___Employee_Management_Console_App.Models;
using Task_03___Employee_Management_Console_App.Services;
using Task_03___Employee_Management_Console_App.Validations;
using TTask_03___Employee_Management_Console_App.Validations;

namespace Task_03___Employee_Management_Console_App.UI
{
    public class ConsoleMenu
    {
        private readonly EmployeeService _employeeService;
        private readonly EmployeeReportService _reportService;

        public ConsoleMenu(EmployeeService employeeService, EmployeeReportService reportService)
        {
            _employeeService = employeeService;
            _reportService = reportService;
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                DisplayMainMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddEmployeeFlow();
                        break;
                    case "2":
                        UpdateEmployeeFlow();
                        break;
                    case "3":
                        DeactivateEmployeeFlow();
                        break;
                    case "4":
                        SearchEmployeeFlow();
                        break;
                    case "5":
                        FilterByDepartmentFlow();
                        break;
                    case "6":
                        SortEmployeesFlow();
                        break;
                    case "7":
                        DisplaySalaryReport();
                        break;
                    case "8":
                        ViewAllEmployees();
                        break;
                    case "9":
                        isRunning = false;
                        Console.WriteLine("\nThank you for using Employee Management System. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("\nError: Invalid option. Please choose a number between 1 and 9.\n");
                        break;
                }
            }
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("   EMPLOYEE MANAGEMENT SYSTEM");
            Console.WriteLine("========================================\n");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Update Employee");
            Console.WriteLine("3. Deactivate Employee");
            Console.WriteLine("4. Search Employee");
            Console.WriteLine("5. Filter by Department");
            Console.WriteLine("6. Sort Employees");
            Console.WriteLine("7. Show Salary Reports");
            Console.WriteLine("8. View All Employees");
            Console.WriteLine("9. Exit");
            Console.Write("\nChoose an option: ");
        }

        private void AddEmployeeFlow()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("          ADD NEW EMPLOYEE");
            Console.WriteLine("========================================\n");

            string fullName, email, phoneNumber, department, position;
            decimal salary;
            DateTime hireDate;

            do
            {
                fullName = GetInput("Enter full name: ");
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    DisplayError("Name is required and cannot be empty! Try again.");
                    continue;
                }
                break;
            } while (true);

            do
            {
                email = GetInput("Enter email: ");
                var eResult = EmailValidator.Validate(email);
                if (!eResult.IsValid)
                {
                    DisplayError(eResult.ErrorMessage);
                    continue;
                }
                break;
            } while (true);

            do
            {
                phoneNumber = GetInput("Enter phone number: ");
                var pResult = PhoneValidator.Validate(phoneNumber);
                if (!pResult.IsValid)
                {
                    DisplayError(pResult.ErrorMessage);
                    continue;
                }
                phoneNumber = pResult.NormalizedNumber;
                break;
            } while (true);

            do
            {
                department = GetInput("Enter department: ");
                if (string.IsNullOrWhiteSpace(department))
                {
                    DisplayError("Department is required and can not be empty! Try again.");
                    continue;
                }
                break;
            } while (true);

            do
            {
                position = GetInput("Enter position: ");
                if (string.IsNullOrWhiteSpace(position))
                {
                    DisplayError("Position is required and can not be empty! Try again.");
                    continue;
                }
                break;
            } while (true);

            do
            {
                Console.Write("Enter salary: ");
                if (!decimal.TryParse(Console.ReadLine(), out salary) || salary <= 0)
                {
                    DisplayError("Error: Invalid salary format. Please enter a valid number.\n");
                    continue;
                }
                break;
            } while (true);

            do
            {
                Console.Write("Enter hire date (yyyy-MM-dd): ");
                if (!DateTime.TryParse(Console.ReadLine(), out hireDate))
                {
                    DisplayError("Error: Invalid date format. Please use yyyy-MM-dd.\n");
                    continue;
                }
                if (hireDate > DateTime.Now)
                {
                    DisplayError("Error: Hire date cannot be in the future.");
                    continue;
                }
                break;
            } while (true);

            var result = _employeeService.AddEmployee(fullName, email, department,phoneNumber, position, salary, hireDate);

            Console.WriteLine(result.Message);
        }

        private void UpdateEmployeeFlow()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("       UPDATE EMPLOYEE");
            Console.WriteLine("========================================\n");

            Console.Write("Enter Employee ID to update: ");
            string employeeId = Console.ReadLine();

            var employee = _employeeService.SearchByEmployeeId(employeeId).FirstOrDefault();
            if (employee == null)
            {
                DisplayError($"\nError: Employee with ID {employeeId} was not found.\n");
                return;
            }

            Console.WriteLine($"\nCurrent Details:");
            Console.WriteLine($"  Name: {employee.FullName}");
            Console.WriteLine($"  Email: {employee.Email}");
            Console.WriteLine($"  Department: {employee.Department}");
            Console.WriteLine($"  Position: {employee.Position}");
            Console.WriteLine($"  Salary: ${employee.Salary:N2}\n");

            Console.WriteLine("Enter new values (press Enter to keep current value):\n");

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Phone: ");
            string phone = Console.ReadLine();

            Console.Write("Department: ");
            string department = Console.ReadLine();

            Console.Write("Position: ");
            string position = Console.ReadLine();

            Console.Write("Salary: ");
            decimal? salary = null;
            string salaryInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(salaryInput))
            {
                if (decimal.TryParse(salaryInput, out decimal parsedSalary))
                {
                    salary = parsedSalary;
                }
                else
                {
                    DisplayError("\nError: Invalid salary format.\n");
                    return;
                }
            }

            var result = _employeeService.UpdateEmployee(employeeId, email, phone, department, position, salary);

            if(result.Success) 
                DisplaySuccess(result.Message);
            else
                DisplayError(result.Message);
        }

        private void DeactivateEmployeeFlow()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("       DEACTIVATE EMPLOYEE");
            Console.WriteLine("========================================\n");

            Console.Write("Enter Employee ID to deactivate: ");
            string employeeId = Console.ReadLine()?.ToUpper();

            var result = _employeeService.DeactivateEmployee(employeeId);

            if (result.Success)
                DisplaySuccess(result.Message);
            else
                DisplayError(result.Message);
        }

        private void SearchEmployeeFlow()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("        SEARCH EMPLOYEE");
            Console.WriteLine("========================================\n");

            Console.WriteLine("1. Search by Employee ID");
            Console.WriteLine("2. Search by Name");
            Console.Write("\nChoose search type: ");
            string choice = Console.ReadLine();

            List<Employee> results = new List<Employee>();

            if (choice == "1")
            {
                Console.Write("Enter Employee ID: ");
                string employeeId = Console.ReadLine()?.ToUpper();
                results = _employeeService.SearchByEmployeeId(employeeId);
            }
            else if (choice == "2")
            {
                Console.Write("Enter employee name (partial match supported): ");
                string name = Console.ReadLine();
                results = _employeeService.SearchByName(name);
            }
            else
            {
                DisplayError("Error: Invalid choice.\n");
                return;
            }

            Console.WriteLine();

            if (results.Count == 0)
            {
                Console.WriteLine("No employees found matching your search criteria.\n");
            }
            else
            {
                DisplayEmployeeTable(results);
            }

        }

        private void FilterByDepartmentFlow()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("    FILTER EMPLOYEES BY DEPARTMENT");
            Console.WriteLine("========================================\n");

            Console.Write("Enter department name: ");
            string department = Console.ReadLine();

            var results = _employeeService.FilterByDepartment(department);

            Console.WriteLine();

            if (results.Count == 0)
            {
                Console.WriteLine($"No active employees found in the {department} department.\n");
            }
            else
            {
                Console.WriteLine($"Employees in {department} department ({results.Count} found):\n");
                DisplayEmployeeTable(results);
            }

        }

        private void SortEmployeesFlow()
        {
            bool inSortMenu = true;

            while (inSortMenu)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("         SORT EMPLOYEES");
                Console.WriteLine("========================================\n");

                Console.WriteLine("1. Salary Ascending (Low to High)");
                Console.WriteLine("2. Salary Descending (High to Low)");
                Console.WriteLine("3. Hire Date Ascending (Oldest to Newest)");
                Console.WriteLine("4. Hire Date Descending (Newest to Oldest)");
                Console.WriteLine("5. Name (Alphabetical)");
                Console.WriteLine("6. Back to Menu");
                Console.Write("\nChoose sorting option: ");
                string choice = Console.ReadLine();

                List<Employee> sortedEmployees = new List<Employee>();

                switch (choice)
                {
                    case "1":
                        sortedEmployees = _employeeService.SortBySalaryAscending();
                        Console.WriteLine("\nEmployees sorted by salary (ascending):\n");
                        DisplayEmployeeTable(sortedEmployees);
                        break;
                    case "2":
                        sortedEmployees = _employeeService.SortBySalaryDescending();
                        Console.WriteLine("\nEmployees sorted by salary (descending):\n");
                        DisplayEmployeeTable(sortedEmployees);
                        break;
                    case "3":
                        sortedEmployees = _employeeService.SortByHireDateAscending();
                        Console.WriteLine("\nEmployees sorted by hire date (oldest to newest):\n");
                        DisplayEmployeeTable(sortedEmployees);
                        break;
                    case "4":
                        sortedEmployees = _employeeService.SortByHireDateDescending();
                        Console.WriteLine("\nEmployees sorted by hire date (newest to oldest):\n");
                        DisplayEmployeeTable(sortedEmployees);
                        break;
                    case "5":
                        sortedEmployees = _employeeService.SortByName();
                        Console.WriteLine("\nEmployees sorted by name (alphabetical):\n");
                        DisplayEmployeeTable(sortedEmployees);
                        break;
                    case "6":
                        inSortMenu = false;
                        break;
                    default:
                        DisplayError("Error: Invalid option. Please choose a number between 1 and 6.\n");
                        break;
                }

            }
        }

        private void DisplaySalaryReport()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("          SALARY REPORT");
            Console.WriteLine("========================================\n");

            decimal avgSalary = _reportService.GetAverageSalary();
            decimal totalPayroll = _reportService.GetTotalPayroll();
            var highestEmployee = _reportService.GetHighestSalaryEmployee();
            var lowestEmployee = _reportService.GetLowestSalaryEmployee();
            var departmentCounts = _reportService.GetEmployeeCountByDepartment();
            int activeCount = _reportService.GetActiveEmployeeCount();
            int inactiveCount = _reportService.GetInactiveEmployeeCount();

            Console.WriteLine($"Average Salary: ${avgSalary:N2}");
            Console.WriteLine($"Total Payroll: ${totalPayroll:N2}\n");

            if (highestEmployee != null)
            {
                Console.WriteLine("Highest Salary:");
                Console.WriteLine($"  {highestEmployee.FullName} - ${highestEmployee.Salary:N2}\n");
            }

            if (lowestEmployee != null)
            {
                Console.WriteLine("Lowest Salary:");
                Console.WriteLine($"  {lowestEmployee.FullName} - ${lowestEmployee.Salary:N2}\n");
            }

            Console.WriteLine("Employees by Department:");
            foreach (var dept in departmentCounts)
            {
                Console.WriteLine($"  {dept.Key,-15}: {dept.Value}");
            }

            Console.WriteLine($"\nEmployee Status:");
            Console.WriteLine($"  Active  : {activeCount}");
            Console.WriteLine($"  Inactive: {inactiveCount}");

            Console.WriteLine();
        }

        private void ViewAllEmployees()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("        ALL EMPLOYEES");
            Console.WriteLine("========================================\n");

            var employees = _employeeService.GetAllEmployees();

            if (employees.Count == 0)
            {
                Console.WriteLine("No employees found.\n");
            }
            else
            {
                DisplayEmployeeTable(employees);
            }

        }

        private void DisplayEmployeeTable(List<Employee> employees)
        {
            if (employees.Count == 0)
            {
                Console.WriteLine("No employees to display.\n");
                return;
            }

            Console.WriteLine(new string('-', 120));

            Console.WriteLine("{0,-10} {1,-20} {2,-20} {3,-12} {4,-20} {5,-12} {6,-12} {7,-10}",
                "ID", "Name", "Email", "Department", "Position", "Salary", "Hire Date", "Status");

            Console.WriteLine(new string('-', 120));

            foreach (var emp in employees)
            {
                Console.WriteLine("{0,-10} {1,-20} {2,-20} {3,-12} {4,-20} {5,-12} {6,-12} {7,-10}",
                    emp.EmployeeId,
                    emp.FullName,
                    emp.Email,
                    emp.Department,
                    emp.Position,
                    $"${emp.Salary:N0}",
                    emp.HireDate.ToString("yyyy-MM-dd"),
                    emp.IsActive ? "Active" : "Inactive");
            }
            Console.WriteLine(new string('-', 120));
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
        private string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }
    }
}
