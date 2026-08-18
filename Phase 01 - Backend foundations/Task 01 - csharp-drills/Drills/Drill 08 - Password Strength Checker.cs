namespace Task_01_csharp_drills.Drills
{
    internal class Drill_08_Password_Strength_Checker
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("||  Password Strength Checker         ||");
            Console.WriteLine("=======================================\n");

            do
            {
                Console.Write("Enter a password: ");
                var password = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Error: Please provide a password.");
                    continue;
                }

                List<string> missingRequirements = new List<string>();

                if (password.Length < 8)
                    missingRequirements.Add("At least 8 characters");

                if (!password.Any(char.IsUpper))
                    missingRequirements.Add("uppercase");

                if (!password.Any(char.IsLower))
                    missingRequirements.Add("lowercase");

                if (!password.Any(char.IsDigit))
                    missingRequirements.Add("digit");

                if (!password.Any(c => !char.IsLetterOrDigit(c)))
                    missingRequirements.Add("special character");

                if (missingRequirements.Count == 0)
                {
                    Console.WriteLine("Strong");
                }
                else
                {
                    Console.Write("Weak . Missing:");
                    foreach (var requirement in missingRequirements)
                    {
                        Console.Write($"  - {requirement}");
                    }
                }
                Console.WriteLine();
                break;
            } while (true);
        }
    }
}