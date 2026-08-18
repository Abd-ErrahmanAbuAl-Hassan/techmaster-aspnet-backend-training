using System;

namespace Task_01_csharp_drills.Drills
{
    internal class Drill_03_Simple_Login_Validator
    {
        public static void Run()
        {
            string _username = "admin";
            string _password = "Admin#123";

            Console.WriteLine("=============================");
            Console.WriteLine("||     Login Validator     ||");
            Console.WriteLine("=============================\n");

            for (int i = 3 ; i > 0; i--)
            {
                bool hasValue = false;
                string? username;
                string? password;

                do
                {
                    Console.Write("Username:");
                    username = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(username))
                    {
                        Console.WriteLine("Error: username not entered! Try again");
                        continue;
                    } 

                    hasValue = true;

                } while (!hasValue);

                hasValue = false;

                do
                {
                    Console.Write("password:");
                    password = Console.ReadLine();


                    if (string.IsNullOrWhiteSpace(password))
                    {
                        Console.WriteLine("Error: password not entered! Try again");
                        continue;
                    } 

                    hasValue = true;

                } while (!hasValue);

                if (!username.Equals(_username, StringComparison.OrdinalIgnoreCase) || !password.Equals(_password))
                {
                    if(i - 1 == 0)
                    {
                        Console.WriteLine("Account locked. Too many failed attempts.\n");
                        return;
                    }
                    Console.WriteLine("Invalid username or password.");
                    Console.WriteLine($"remaining attempt {i - 1}.\n");
                }
                else { Console.WriteLine("Successful login."); break; }
            }



        }
    }
}
