namespace Task_01_csharp_drills.Drills
{
    internal class Drill_01_Temperature_Converter
    {
        public static void Run()
        {

            Console.WriteLine("=============================");
            Console.WriteLine("||  Temperature Converter  ||");
            Console.WriteLine("=============================\n");
            Console.Write("Enter Temperature in Celsius:");

            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("Error: Temperature not entered!");
                return;
            }

            double celsius, fahrenheit;
            if (!double.TryParse(userInput, out celsius))
            {
                Console.WriteLine("Error: Invalid temperature value.");
                return;
            }

            fahrenheit = (celsius * 9 / 5) + 32;

            Console.WriteLine($"Fahrenheit Temperature : {fahrenheit:F2}F");

        }
    }
}
