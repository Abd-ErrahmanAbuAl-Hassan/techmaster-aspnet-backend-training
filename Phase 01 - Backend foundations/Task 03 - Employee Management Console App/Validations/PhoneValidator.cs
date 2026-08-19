using System.Text.RegularExpressions;

namespace TTask_03___Employee_Management_Console_App.Validations
{
    internal class PhoneValidator
    {
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
            public string NormalizedNumber { get; set; }
        }


        private static readonly string[] MobilePrefixes = { "010", "011", "012", "015" };

        public static ValidationResult Validate(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Phone number is required."
                };

            string cleaned = phone.StartsWith("+20") ? "0" + phone.Substring(3) : phone;

            if (cleaned.Length != 11)
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Phone number must be between 11 digits."
                };

            // Check if it contains only digits
            if (!Regex.IsMatch(cleaned, @"^\d+$"))
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Phone number must contain only digits."
                };

            if (IsMobileNumber(cleaned))
            {
                return new ValidationResult
                {
                    IsValid = true,
                    NormalizedNumber = cleaned,
                    ErrorMessage = null
                };
            }

            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Invalid Egyptian phone number format. Use 01X XXXX XXXX.",
            };
        }

        private static bool IsMobileNumber(string phone)
        {
            if (!phone.StartsWith("01"))
                return false;

            // Check prefix (010, 011, 012, 015)
            string prefix = phone.Substring(0, 3);
            return Array.Exists(MobilePrefixes, p => p == prefix);
        }

    }
}
