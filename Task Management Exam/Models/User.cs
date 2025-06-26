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
    public Enums.UserRole Role { get; set; } = Enums.UserRole.Regular;
}
