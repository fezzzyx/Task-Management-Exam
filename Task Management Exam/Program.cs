using Task_Management_Exam.Data;
using Task_Management_Exam.Models;
using Task_Management_Exam.Services;

class Program
{
    static void Main()
    {
        using var db = new AppDbContext();
        var userService = new UserService(db);

        bool running = true;

        while (running)
        {
            Console.WriteLine("\n=== MENU ===");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. View Profile");
            Console.WriteLine("4. Logout");
            Console.WriteLine("5. Exit Program");
            Console.Write("Select an option: ");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.Write("Username: ");
                    string? regUsername = Console.ReadLine();

                    Console.Write("Password: ");
                    string? regPassword = Console.ReadLine();

                    Console.Write("Role (admin/user): ");
                    string? roleInput = Console.ReadLine();
                    UserRole role = roleInput?.ToLower() == "admin" ? UserRole.Admin : UserRole.Regular;

                    userService.Register(regUsername, regPassword, role);
                    break;

                case "2":
                    Console.Write("Username: ");
                    string? loginUsername = Console.ReadLine();

                    Console.Write("Password: ");
                    string? loginPassword = Console.ReadLine();

                    userService.Login(loginUsername, loginPassword);
                    break;

                case "3":
                    userService.ViewProfile();
                    break;

                case "4":
                    userService.Logout();
                    break;

                case "5":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        Console.WriteLine("Program exited.");
    }
}

