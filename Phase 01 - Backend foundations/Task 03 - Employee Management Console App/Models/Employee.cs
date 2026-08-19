namespace Task_03___Employee_Management_Console_App.Models
{
    public class Employee
    {
        public string EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public string PhoneNumber { get; set; }
        public string ManagerName { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"{EmployeeId} | {FullName} | {Email} | {Department} | {Position} | ${Salary:N2} | {HireDate:yyyy-MM-dd} | {(IsActive ? "Active" : "Inactive")}";
        }
    }
}
