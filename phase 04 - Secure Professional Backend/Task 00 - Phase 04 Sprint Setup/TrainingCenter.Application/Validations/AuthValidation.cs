using System.Text.RegularExpressions;
using TrainingCenter.Application.DTOs.Auth.Requests;

namespace TrainingCenter.Application.Validations
{
    public class AuthValidation
    {
        public static List<string> RegisterValidate(RegisterRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.FirstName))
                errors.Add("First name is required.");

            if (string.IsNullOrWhiteSpace(request.LastName))
                errors.Add("Last name is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required.");
            else if (!IsValidEmail(request.Email))
                errors.Add("Email format is invalid.");

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                errors.Add("Phone number is required.");
            else if (!IsValidPhoneNumber(request.PhoneNumber))
                errors.Add("Phone number format is invalid.");

            if (string.IsNullOrWhiteSpace(request.Password))
                errors.Add("Password is required.");
            else if (string.IsNullOrWhiteSpace(request.ConfirmPassword))
                errors.Add("Confirm Password is required.");
            else if(request.Password != request.ConfirmPassword)
                errors.Add("Password doesn't match.");

            return errors;
        }
        public static List<string> LoginValidate(LoginRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required.");
            else if (!IsValidEmail(request.Email))
                errors.Add("Email format is invalid.");

            if (string.IsNullOrWhiteSpace(request.Password))
                errors.Add("Password is required.");

            return errors;
        }
        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^01[0125]\d{8}$");
        }
    }
}
