using Task_03___Employee_Management_Console_App.Models;

namespace Task_03___Employee_Management_Console_App.Services
{
    public class EmployeeReportService
    {
        private readonly EmployeeService _employeeService;

        public EmployeeReportService(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public decimal GetAverageSalary()
        {
            var employees = _employeeService.GetAllEmployees();
            if (employees.Count == 0)
                return 0;

            return employees.Average(e => e.Salary);
        }

        public Employee GetHighestSalaryEmployee()
        {
            var employees = _employeeService.GetAllEmployees();
            if (employees.Count == 0)
                return null;

            return employees.MaxBy(e => e.Salary);
        }

        public Employee GetLowestSalaryEmployee()
        {
            var employees = _employeeService.GetAllEmployees();
            if (employees.Count == 0)
                return null;

            return employees.MinBy(e => e.Salary);
        }

        public decimal GetTotalPayroll()
        {
            var employees = _employeeService.GetAllEmployees();
            return employees.Sum(e => e.Salary);
        }

        public Dictionary<string, int> GetEmployeeCountByDepartment()
        {
            var employees = _employeeService.GetAllEmployees();
            
            return employees
                .GroupBy(e => e.Department)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public int GetActiveEmployeeCount()
        {
            var employees = _employeeService.GetAllEmployees();
            return employees.Count(e => e.IsActive);
        }

        public int GetInactiveEmployeeCount()
        {
            var employees = _employeeService.GetAllEmployees();
            return employees.Count(e => !e.IsActive);
        }


        public (decimal Average, decimal Total, int Count) GetDepartmentSalaryStats(string department)
        {
            var employees = _employeeService.GetAllEmployees()
                .Where(e => e.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (employees.Count == 0)
                return (0, 0, 0);

            return (
                Average: employees.Average(e => e.Salary),
                Total: employees.Sum(e => e.Salary),
                Count: employees.Count
            );
        }
    }
}
