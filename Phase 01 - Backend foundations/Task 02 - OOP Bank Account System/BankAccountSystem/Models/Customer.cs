namespace Task_02_OOP_Bank_Account_System.BankAccountSystem.Models
{
    public class Customer
    {
        public string CustomerId { get; }
        public string FullName { get; }
        public string Email { get; }
        public string PhoneNumber { get; }
        public DateTime CreatedAt { get; }

        public Customer(string fullName, string email, string phoneNumber)
        {
            // Generate unique customer ID
            CustomerId = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            CreatedAt = DateTime.Now;
        }
    }
}