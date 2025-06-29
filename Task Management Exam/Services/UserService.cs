
using Microsoft.EntityFrameworkCore;
using Task_Management_Exam.Models;
using Task_Management_Exam.Data;
using Microsoft.Extensions.Configuration;
using UserRole = Task_Management_Exam.Models.UserRole;


namespace Task_Management_Exam.Services;

public class UserService
{
    private readonly AppDbContext db;
    private User? currentUser;

    public UserService(AppDbContext context)
    {
        db = context;
        db.Database.EnsureCreated(); 
    }

    public bool Register(string username, string password, UserRole role = UserRole.Regular)
    {
        if (db.Users.Any(u => u.Username == username))
        {
            Console.WriteLine("User already exists.");
            return false;
        }

        var user = new User
        {
            Username = username,
            Password = password,
            UserRole = role
        };

        db.Users.Add(user);
        db.SaveChanges();
        Console.WriteLine("Registration successful.");
        return true;
    }

    public bool Login(string username, string password)
    {
        var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user == null)
        {
            Console.WriteLine("Invalid username or password.");
            return false;
        }

        currentUser = user;
        Console.WriteLine($"Login successful. Welcome, {user.Username} ({user.UserRole})");
        return true;
    }

    public void Logout()
    {
        currentUser = null;
        Console.WriteLine("Logged out successfully.");
    }

    public void ViewProfile()
    {
        if (currentUser == null)
        {
            Console.WriteLine("Please log in to view profile.");
            return;
        }

        Console.WriteLine("\n=== Profile ===");
        Console.WriteLine($"Username: {currentUser.Username}");
        Console.WriteLine($"Role: {currentUser.UserRole}");
    }

    public bool IsAdmin() => currentUser?.UserRole == UserRole.Admin;

    public bool IsLoggedIn() => currentUser != null;
}





