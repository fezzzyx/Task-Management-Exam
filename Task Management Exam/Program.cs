using Task_Management_Exam.Data;
using Task_Management_Exam.Models;
using Task_Management_Exam.Services;

var context = new AppDbContext();
var userService = new UserService(context);

User? currentUser = null;

while (true)
{
    Console.Clear();

    if (currentUser == null)
    {
        Console.WriteLine("=== Task Management ===");
        Console.WriteLine("1. Register");
        Console.WriteLine("2. Login");
        Console.WriteLine("0. Exit");
        Console.Write("Choose option: ");
        var authInput = Console.ReadLine();

        switch (authInput)
        {
            case "1":
                Console.Write("New Username: ");
                var newUser = Console.ReadLine();
                Console.Write("New Password: ");
                var newPass = Console.ReadLine();
                var registered = await userService.RegisterAsync(newUser, newPass);
                Console.WriteLine(registered ? "Registered!" : "Username taken.");
                break;

            case "2":
                Console.Write("Username: ");
                var loginUser = Console.ReadLine();
                Console.Write("Password: ");
                var loginPass = Console.ReadLine();
                var loggedIn = await userService.LoginAsync(loginUser, loginPass);
                if (loggedIn != null)
                {
                    currentUser = loggedIn;
                    Console.WriteLine($"Welcome, {currentUser.Username} ({currentUser.UserRole})!");
                }
                else Console.WriteLine("Invalid login.");
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Invalid option.");
                break;
        }

        Console.WriteLine("\nPress Enter...");
        Console.ReadLine();
        continue;
    }
    Console.Clear();
    Console.WriteLine($"Logged in as: {currentUser.Username} ({currentUser.UserRole})");
    Console.WriteLine("=== Main Menu ===");

    Console.WriteLine("\nPress Enter to continue...");
    Console.ReadLine();
}


