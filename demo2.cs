using System;

namespace LoginSystem
{
    class UserAccount
    {
        private string username;
        private string password;

        public UserAccount(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

       
        public void ShowUserInfo()
        {
            Console.WriteLine("User: " + username);
            Console.WriteLine("Account Status: Active");
        }
    }

    class DemoLogin
    {
        static void Main()
        {
            UserAccount user = new UserAccount("student01", "pass123");

            Console.WriteLine("=== Login System Demo ===");

            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            if (user.Login(username, password))
            {
                Console.WriteLine("\nLogin Successful!");
                user.ShowUserInfo();
            }
            else
            {
                Console.WriteLine("\nLogin Failed!");
                Console.WriteLine("Invalid username or password.");
            }
        }
    }
}
