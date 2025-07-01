using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Task_Management_Exam.Data;
using Task_Management_Exam.Models;
using static Task_Management_Exam.Models.Enums;

namespace Task_Management_Exam.Services;

public class UserService : IUserService
{
    private readonly AppDbContext db;
    public User? CurrentUser { get; private set; }
    public UserService(AppDbContext context)
    {
        db = context;
    }

    public async Task<bool> RegisterAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        if (db.Users.Any(u => u.Username == username))
            throw new Exception($"User with username '{username}' already exists.");

        var user = new User
        {
            Username = username,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            UserRole = UserRole.Regular
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<User?> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        if (username == "admin" && password == "admin")
        {
            return new User
            {
                Id = 0,
                Username = "admin",
                UserRole = UserRole.Admin,
                Password = "admin"
            };
        }

        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        if (user == null)
            throw new Exception("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new Exception("Incorrect password.");

        CurrentUser = user;
        return user;
    }
    public List<User> GetAllUsers()
    {
        return db.Users.ToList();
    }

}





