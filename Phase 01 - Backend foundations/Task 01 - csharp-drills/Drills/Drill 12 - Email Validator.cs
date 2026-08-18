namespace Task_01_csharp_drills.Drills
{
    internal class Drill_12_Email_Validator
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("||      Email Validator             ||");
            Console.WriteLine("=======================================\n");

            do
            {
                Console.Write("Enter an email address: ");
                var email = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Error: Please provide an email.");
                    continue;
                }

                bool isValid = IsValidEmail(email);

                if (isValid)
                {
                    Console.WriteLine("Valid\n");
                }
                else
                {
                    Console.WriteLine("Invalid\n");
                }
                break;
            } while (true);
        }

        private static bool IsValidEmail(string email)
        {
            email = email.Trim();

            if (!email.Contains("@") || email.IndexOf("@") != email.LastIndexOf("@") || email.StartsWith("@") || email.EndsWith("@"))
                return false;

            var parts = email.Split('@');
            if (parts.Length != 2)
                return false;

            var localPart = parts[0];
            var domain = parts[1];

            if (string.IsNullOrWhiteSpace(localPart) || string.IsNullOrWhiteSpace(domain))
                return false;

            if(localPart.Length < 3) 
                return false;

            if (!domain.Contains("."))
                return false;

            var domainParts = domain.Split('.');
            foreach (var part in domainParts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    return false;
            }

            return true;
        }
    }
}