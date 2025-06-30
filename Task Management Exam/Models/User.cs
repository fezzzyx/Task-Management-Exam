using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Management_Exam.Models;


public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public Enums.UserRole UserRole { get; set; } = Enums.UserRole.Regular;
    public string? Email { get; set; }
}

