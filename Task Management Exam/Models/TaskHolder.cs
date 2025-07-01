using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Management_Exam.Models;

public class TaskHolder
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.ToDo;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime Deadline { get; set; }

    public int? UserId { get; set; }
    public User? AssignedUser { get; set; }
}
