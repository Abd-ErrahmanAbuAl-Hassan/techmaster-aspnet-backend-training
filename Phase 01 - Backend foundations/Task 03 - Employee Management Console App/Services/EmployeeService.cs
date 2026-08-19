using Task_03___Employee_Management_Console_App.Models;

namespace Task_03___Employee_Management_Console_App.Services
{
    public class EmployeeService
    {
        private List<Employee> employees;

        public EmployeeService()
        {
            employees = new List<Employee>();
            InitializeSeedData();
        }

        private void InitializeSeedData()
        {
            employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = "EMP-001",
                    FullName = "Mohamed Ayman",
                    Email = "mohamed@test.com",
                    Department = "IT",
                    Position = "Backend Developer",
                    Salary = 20000,
                    HireDate = new DateTime(2025, 1, 10),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Employee
                {
                    EmployeeId = "EMP-002",
                    FullName = "Sara Adel",
                    Email = "sara@test.com",
                    Department = "HR",
                    Position = "HR Specialist",
                    Salary = 12000,
                    HireDate = new DateTime(2024, 5, 15),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Employee
                {
                    EmployeeId = "EMP-003",
                    FullName = "Ahmed Tarek",
                    Email = "ahmed@test.com",
                    Department = "IT",
                    Position = "Junior Developer",
                    Salary = 9000,
                    HireDate = new DateTime(2026, 1, 1),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Employee
                {
                    EmployeeId = "EMP-004",
                    FullName = "Omar Samir",
                    Email = "omar@test.com",
                    Department = "Sales",
                    Position = "Sales Executive",
                    Salary = 11000,
                    HireDate = new DateTime(2023, 11, 20),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Employee
                {
                    EmployeeId = "EMP-005",
                    FullName = "Mariam Hassan",
                    Email = "mariam@test.com",
                    Department = "Finance",
                    Position = "Accountant",
                    Salary = 14000,
                    HireDate = new DateTime(2022, 9, 11),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Employee
                {
                    EmployeeId = "EMP-006",
                    FullName = "Khaled Ali",
                    Email = "khaled@test.com",
                    Department = "IT",
                    Position = "DevOps Trainee",
                    Salary = 10000,
                    HireDate = new DateTime(2026, 2, 1),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Employee
                {
                    EmployeeId = "EMP-007",
                    FullName = "Nour Emad",
                    Email = "nour@test.com",
                    Department = "Marketing",
                    Position = "Content Specialist",
                    Salary = 9500,
                    HireDate = new DateTime(2025, 7, 8),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Employee
                {
                    EmployeeId = "EMP-008",
                    FullName = "Youssef Nabil",
                    Email = "youssef@test.com",
                    Department = "Sales",
                    Position = "Sales Manager",
                    Salary = 18000,
                    HireDate = new DateTime(2021, 3, 17),
                    IsActive = false,
                    CreatedAt = DateTime.Now
                },
                new Employee
                {
                    EmployeeId = "EMP-009",
                    FullName = "Dina Farouk",
                    Email = "dina@test.com",
                    Department = "HR",
                    Position = "Recruiter",
                    Salary = 10500,
                    HireDate = new DateTime(2024, 2, 13),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Employee
                {
                    EmployeeId = "EMP-010",
                    FullName = "Hady Mahmoud",
                    Email = "hady@test.com",
                    Department = "IT",
                    Position = "QA Engineer",
                    Salary = 13000,
                    HireDate = new DateTime(2025, 10, 1),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Employee
                {
                    EmployeeId = "EMP-011",
                    FullName = "Salma Taha",
                    Email = "salma@test.com",
                    Department = "Finance",
                    Position = "Finance Manager",
                    Salary = 26000,
                    HireDate = new DateTime(2020, 12, 12),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Employee
                {
                    EmployeeId = "EMP-012",
                    FullName = "Ali Mostafa",
                    Email = "ali@test.com",
                    Department = "Support",
                    Position = "Support Agent",
                    Salary = 8000,
                    HireDate = new DateTime(2026, 3, 5),
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
            };
        }

        public List<Employee> GetAllEmployees()
        {
            return new List<Employee>(employees);
        }

        public List<Employee> GetActiveEmployees()
        {
            return employees.Where(e => e.IsActive).ToList();
        }

        public (bool Success, string Message) AddEmployee(string fullName, string email,string phone, string department, string position, decimal salary, DateTime hireDate)
        {
            string employeeId = GenerateUniqueEmployeeId();

            var newEmployee = new Employee
            {
                EmployeeId = employeeId,
                FullName = fullName.Trim(),
                Email = email.Trim(),
                PhoneNumber = phone.Trim(),
                Department = department.Trim(),
                Position = position.Trim(),
                Salary = salary,
                HireDate = hireDate,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            employees.Add(newEmployee);
            return (true, $"Success: Employee {employeeId} ({fullName}) has been added successfully.");
        }

        public (bool Success, string Message) UpdateEmployee(string employeeId, string email = null, string phone = null,

            string department = null, string position = null, decimal? salary = null)
        {
            var employee = employees.FirstOrDefault(e => e.EmployeeId == employeeId);

            if (employee == null)
                return (false, $"Error: Employee with ID {employeeId} was not found.");

            if (!string.IsNullOrWhiteSpace(email))
            {
                employee.Email = email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                employee.PhoneNumber = phone.Trim();
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                employee.Department = department.Trim();
            }

            if (!string.IsNullOrWhiteSpace(position))
            {
                employee.Position = position.Trim();
            }

            if (salary.HasValue)
            {
                if (salary.Value <= 0)
                    return (false, "Error: Salary must be greater than zero.");
                employee.Salary = salary.Value;
            }

            return (true, $"Success: Employee {employeeId} ({employee.FullName}) has been updated successfully.");
        }

        public (bool Success, string Message) DeactivateEmployee(string employeeId)
        {
            var employee = employees.FirstOrDefault(e => e.EmployeeId == employeeId);

            if (employee == null)
                return (false, $"Error: Employee with ID {employeeId} was not found.");

            if (!employee.IsActive)
                return (false, $"Error: Employee {employeeId} is already inactive.");

            employee.IsActive = false;
            return (true, $"Success: Employee {employeeId} ({employee.FullName}) has been deactivated.");
        }

        public List<Employee> SearchByEmployeeId(string employeeId)
        {
            return employees.Where(e => e.EmployeeId == employeeId).ToList();
        }

        public List<Employee> SearchByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Employee>();

            return employees
                .Where(e => e.FullName.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Employee> FilterByDepartment(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                return new List<Employee>();

            return employees
                .Where(e => e.IsActive && e.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Employee> SortBySalaryAscending()
        {
            return employees.OrderBy(e => e.Salary).ToList();
        }

        public List<Employee> SortBySalaryDescending()
        {
            return employees.OrderByDescending(e => e.Salary).ToList();
        }

        public List<Employee> SortByHireDateAscending()
        {
            return employees.OrderBy(e => e.HireDate).ToList();
        }

        public List<Employee> SortByHireDateDescending()
        {
            return employees.OrderByDescending(e => e.HireDate).ToList();
        }

        public List<Employee> SortByName()
        {
            return employees.OrderBy(e => e.FullName).ToList();
        }

        private string GenerateUniqueEmployeeId()
        {
            int maxNumber = 0;

            foreach (var employee in employees)
            {
                if (employee.EmployeeId.StartsWith("EMP-"))
                {
                    string numberPart = employee.EmployeeId.Substring(4);
                    if (int.TryParse(numberPart, out int number))
                    {
                        maxNumber = Math.Max(maxNumber, number);
                    }
                }
            }

            return $"EMP-{(maxNumber + 1):D3}";
        }
    }
}
