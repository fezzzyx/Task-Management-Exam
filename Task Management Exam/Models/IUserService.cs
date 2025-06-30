using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Management_Exam.Models;

public interface IUserService
{
    User? CurrentUser { get; }
    Task<bool> RegisterAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<User?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}
