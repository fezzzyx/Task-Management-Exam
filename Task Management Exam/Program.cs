using System;
using Task_Management_Exam.Data;
using Task_Management_Exam.Models;
using Task_Management_Exam.Services;

var context = new AppDbContext();
var userService = new UserService(context);
var taskService = new TaskService(context);
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
    Console.WriteLine("1. View All Tasks");
    Console.WriteLine("2. Create New Task");
    Console.WriteLine("3. Search Task by Title");
    Console.WriteLine("4. Filter by Status");
    Console.WriteLine("5. Filter by User");
    Console.WriteLine("6. Filter by Deadline");
    Console.WriteLine("7. Delete Task");
    if (currentUser.UserRole == Enums.UserRole.Admin)
        Console.WriteLine("8. View All Users");
    Console.WriteLine("9. Logout");
    Console.WriteLine("0. Exit");
    Console.Write("Choose option: ");

    var input = Console.ReadLine();

    switch (input)
    {
        case "1":
            PrintTasks(await taskService.GetAllTasksAsync());
            break;

        case "2":
            Console.Write("Title: ");
            var title = Console.ReadLine();
            Console.Write("Description: ");
            var desc = Console.ReadLine();
            Console.Write("Deadline (yyyy-MM-dd): ");
            DateTime deadline = DateTime.Parse(Console.ReadLine());

            int? userIdToAssign = null;

            if (currentUser.UserRole == Enums.UserRole.Admin)
            {
                Console.Write("Assign to user (UserId or leave empty): ");
                var userIdInput = Console.ReadLine();
                if (int.TryParse(userIdInput, out int parsedId)) userIdToAssign = parsedId;
            }
            else
            {
                userIdToAssign = currentUser.Id;
            }

            var task = new TaskHolder
            {
                Title = title,
                Description = desc,
                Deadline = deadline,
                UserId = userIdToAssign
            };

            await taskService.AddTaskAsync(task);
            Console.WriteLine("Task added.");
            break;

        case "3":
            Console.Write("Search title: ");
            var searchTitle = Console.ReadLine();
            PrintTasks(await taskService.SearchByTitleAsync(searchTitle));
            break;

        case "4":
            Console.WriteLine("Status: 0 = ToDo, 1 = InProgress, 2 = Done");
            var statusInput = Console.ReadLine();
            if (Enum.TryParse<Enums.TaskStatus>(statusInput, out var status))
                PrintTasks(await taskService.FilterByStatusAsync(status));
            else Console.WriteLine("Invalid status.");
            break;

        case "5":
            Console.Write("User ID: ");
            if (int.TryParse(Console.ReadLine(), out int uid))
                PrintTasks(await taskService.FilterByUserAsync(uid));
            else Console.WriteLine("Invalid user ID.");
            break;

        case "6":
            Console.Write("Enter deadline (yyyy-MM-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime d))
                PrintTasks(await taskService.FilterByDeadlineAsync(d));
            else Console.WriteLine("Invalid date.");
            break;

        case "7":
            Console.Write("Enter task ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int delId))
            {
                await taskService.DeleteTaskAsync(delId);
                Console.WriteLine("Deleted if it existed.");
            }
            else Console.WriteLine("Invalid ID.");
            break;

        case "8":
            if (currentUser.UserRole == Enums.UserRole.Admin)
            {
                var users = context.Users.ToList();
                Console.WriteLine("\n--- Users ---");
                foreach (var u in users)
                    Console.WriteLine($"{u.Id}: {u.Username} ({u.UserRole})");
            }
            else Console.WriteLine("Unauthorized.");
            break;

        case "9":
            currentUser = null;
            Console.WriteLine("Logged out.");
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Invalid option.");
            break;
    }

    Console.WriteLine("\nPress Enter to continue...");
    Console.ReadLine();
}

void PrintTasks(List<TaskHolder> tasks)
{
    Console.WriteLine("\n--- Tasks ---");
    foreach (var t in tasks)
    {
        string user = t.AssignedUser != null ? $"(User: {t.AssignedUser.Username})" : "(Unassigned)";
        Console.WriteLine($"{t.Id}. {t.Title} [{t.Status}] Deadline: {t.Deadline:yyyy-MM-dd} {user}");
    }
}


