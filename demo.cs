using System;

namespace LoginSystem
{
    class Demo
    {
        static void Main(string[] args)
        {
            Console.WriteLine("================================");
            Console.WriteLine("     Login System Demo");
            Console.WriteLine("================================");

            string username = "admin";
            string password = "1234";

            Console.WriteLine($"Username: {username}");
            Console.WriteLine("Password: ****");

            if (username == "admin" && password == "1234")
            {
                Console.WriteLine("Login successful!");
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }

            Console.WriteLine("\nDemo completed successfully.");
        }
    }
}
