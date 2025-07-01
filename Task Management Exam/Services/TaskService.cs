using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Task_Management_Exam.Data;
using Task_Management_Exam.Models;

namespace Task_Management_Exam.Services;

public class TaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<TaskHolder>> GetAllTasksAsync()
    {
        var tasks = await _context.Tasks
            .Include(t => t.AssignedUser)
            .ToListAsync();

        return tasks;
    }
    public async Task<List<TaskHolder>> SearchByTitleAsync(string title)
    {
        var matchedTasks = await _context.Tasks
            .Where(t => t.Title.Contains(title))
            .Include(t => t.AssignedUser)
            .ToListAsync();

        return matchedTasks;
    }
    public async Task<List<TaskHolder>> FilterByStatusAsync(Enums.TaskStatus status)
    {
        var filteredTasks = await _context.Tasks
            .Where(t => t.Status == status)
            .Include(t => t.AssignedUser)
            .ToListAsync();

        return filteredTasks;
    }
    public async Task<List<TaskHolder>> FilterByUserAsync(int userId)
    {
        var filteredTasks = await _context.Tasks
            .Where(t => t.UserId == userId)
            .Include(t => t.AssignedUser)
            .ToListAsync();

        return filteredTasks;
    }
    public async Task<List<TaskHolder>> FilterByDeadlineAsync(DateTime deadline)
    {
        var filteredTasks = await _context.Tasks
            .Where(t => t.Deadline.Date == deadline.Date)
            .Include(t => t.AssignedUser)
            .ToListAsync();

        return filteredTasks;
    }
    public async Task<TaskHolder?> GetTaskByIdAsync(int id)
    {
        var task = await _context.Tasks
            .Include(t => t.AssignedUser)
            .FirstOrDefaultAsync(t => t.Id == id);

        return task;
    }

    public async Task AddTaskAsync(TaskHolder task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTaskAsync(TaskHolder task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int id)
    {
        var taskToDelete = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (taskToDelete != null)
        {
            _context.Tasks.Remove(taskToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
