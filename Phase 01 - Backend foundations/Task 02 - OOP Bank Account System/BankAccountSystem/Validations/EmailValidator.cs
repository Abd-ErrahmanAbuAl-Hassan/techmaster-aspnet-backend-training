using System.Net.Mail;

namespace Task_02_OOP_Bank_Account_System.BankAccountSystem.Validations
{
    internal class EmailValidator
    {
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
        }

        public static ValidationResult Validate(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Email is required and cannot be empty."
                };

            email = email.Trim();

            if (email.Length > 254)
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Email address is too long (maximum 254 characters)."
                };

            try
            {
                var address = new MailAddress(email);

                // Ensure it matches exactly name@domain.com format
                if (address.Address != email)
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Invalid email format. Please use 'name@domain.com'."
                    };

                string domain = address.Host;
                if (!domain.Contains('.') || domain.StartsWith('.') || domain.EndsWith('.'))
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Domain part must contain a valid TLD (e.g., .com, .org)."
                    };


                return new ValidationResult
                {
                    IsValid = true,
                    ErrorMessage = null
                };
            }
            catch (FormatException)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Please enter a valid email address (e.g., name@domain.com)."
                };
            }
        }
    }
}
