namespace Task_01_csharp_drills.Drills
{
    internal class Drill_17_Simple_Search_Engine
    {
        public static void Run()
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("||    Simple Search Engine          ||");
            Console.WriteLine("=======================================\n");

            var names = new List<string>
                        {
                            "Ahmed Hassan",
                            "Mohamed Ali",
                            "Omar Mahmoud",
                            "Youssef Ibrahim",
                            "Mostafa Ahmed",
                            "Abdullah Hassan",
                            "Mahmoud Mohamed",
                            "Khaled Ibrahim",
                            "Amr Mahmoud",
                            "Karim Hassan",
                            "Ali Ahmed",
                            "Hassan Mohamed",
                            "Ibrahim Khaled",
                            "Tarek Mahmoud",
                            "Osama Ali",
                            "Mahmoud Hassan",
                            "Mohamed Ibrahim",
                            "Ahmed Mahmoud",
                            "Omar Hassan",
                            "Youssef Ali"
                        };

            do
            {

                Console.Write("Enter search keyword: ");
                var keyword = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    Console.WriteLine("Error: Please provide a keyword.\n");
                    continue;
                }

                var results = names.Where(n => n.ToLower().Contains(keyword.ToLower())).ToList();

                if (results.Count == 0)
                {
                    Console.WriteLine("No results found.\n");
                }
                else
                {
                    Console.Write($"Search Results: {string.Join(",",results)}\n");
                    break;
                }
                
            } while (true);
        }
    }
}