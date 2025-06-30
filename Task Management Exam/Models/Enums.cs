using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Management_Exam.Models;

public class Enums
{
    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }
    public enum UserRole
    {
        Regular,
        Admin
    }

}
