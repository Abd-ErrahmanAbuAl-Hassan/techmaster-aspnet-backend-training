using System.Diagnostics;

namespace Task_01_csharp_drills.Drills
{
    internal class Drill_02_Grade_Calculator
    {
        public static void Run()
        {
            Console.WriteLine("=============================");
            Console.WriteLine("||     GradeCalculator     ||");
            Console.WriteLine("=============================\n");
            Console.Write("Enter the score e.g.(0-100):");

            var userInput = Console.ReadLine();
            if(string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("Error: score not entered!");
                return;
            }
            int score;
            if (!int.TryParse(userInput, out score))
            {
                Console.WriteLine("Error: Invalid score value.");
                return;
            }

            if(score < 0 ||  score > 100)
            {
                Console.WriteLine("Error: Score must be between 0 and 100.");
                return;
            }

            if(score > 89) Console.WriteLine("Grade: A");
            else if(score > 79) Console.WriteLine("Grade: B");
            else if(score > 69) Console.WriteLine("Grade: C");
            else if(score > 59) Console.WriteLine("Grade: D");
            else Console.WriteLine("Grade: F");


        }
    }
}
