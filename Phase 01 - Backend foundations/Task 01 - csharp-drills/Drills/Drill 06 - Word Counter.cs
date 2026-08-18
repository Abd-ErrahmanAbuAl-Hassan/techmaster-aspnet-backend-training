namespace Task_01_csharp_drills.Drills
{
    internal class Drill_06_Word_Counter
    {
        public static void Run()
        {
            Console.WriteLine("===============================");
            Console.WriteLine("||      Word Counter         ||");
            Console.WriteLine("===============================\n");

            do
            {
                Console.Write("Enter a sentence: ");
                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.WriteLine("Error: Sentence cannot be empty.");
                    continue;
                }

                int wordCount = userInput.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;

                Console.WriteLine($"Word Count: {wordCount}\n");
                break;
            } while (true);
        }
    }
}