using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Management_Exam.Models;


public enum UserRole
{
    Regular,
    Admin
}


public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    public UserRole UserRole { get; set; } = UserRole.Regular;
    public string? Email { get; set; }
}

