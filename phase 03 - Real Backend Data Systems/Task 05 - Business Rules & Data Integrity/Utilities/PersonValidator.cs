using System.Text.RegularExpressions;
using Task_05_Business_Rules_Data_Integrity.DTOs.Requests;

namespace Task_05_Business_Rules_Data_Integrity.Utilities
{
    public class PersonValidator
    {
        public static List<string> Validate(CreateStudentRequest request)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(request.FName))
                errors.Add("First name is required.");
            if (string.IsNullOrWhiteSpace(request.LName))
                errors.Add("Last name is required.");
            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required.");
            else if (!IsValidEmail(request.Email))
                errors.Add("Email format is invalid.");
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                errors.Add("Phone number is required.");
            else if (!IsValidPhoneNumber(request.PhoneNumber))
                errors.Add("Phone number format is invalid.");
            if(request is CreateInstructorRequest instructor)
            {
                if (string.IsNullOrWhiteSpace(instructor.Specialization)) errors.Add("Specialization is required.");
                if (string.IsNullOrWhiteSpace(instructor.Bio)) errors.Add("Bio is required.");
            }
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
