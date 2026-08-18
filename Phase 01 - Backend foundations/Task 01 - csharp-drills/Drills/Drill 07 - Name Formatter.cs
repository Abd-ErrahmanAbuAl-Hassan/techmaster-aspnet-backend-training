namespace Task_01_csharp_drills.Drills
{
    internal class Drill_07_Name_Formatter
    {
        public static void Run()
        {
            Console.WriteLine("===============================");
            Console.WriteLine("||      Name Formatter       ||");
            Console.WriteLine("===============================\n");

            do
            {
                Console.Write("Enter a full name: ");
                var userInput = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(userInput) )
                {
                    Console.WriteLine("Error: Please provide a name.");
                    continue;
                }

                if(!userInput.All(char.IsLetter))
                {
                    Console.WriteLine("Error: There is an invalid characters.");
                    continue;
                }

                var names = userInput.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < names.Length; i++)
                {
                    var tempName = names[i].ToLower();
                    names[i] = (char)(tempName[0] - 32) + tempName[1..];
                } 

                Console.WriteLine($"Formatted Name: {string.Join(" ", names)}\n");
                break;
            } while (true);
        }
    }
}