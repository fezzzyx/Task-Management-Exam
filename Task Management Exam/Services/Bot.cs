using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Task_Management_Exam.Models;

namespace Task_Management_Exam.Services;

public class Bot
{
    private readonly DiscordSocketClient _client;
    private readonly IUserService _userService;
    private readonly TaskService _taskService;

    public Bot(DiscordSocketClient client, IUserService userService, TaskService taskService)
    {
        _client = client;
        _userService = userService;
        _taskService = taskService;
    }

    public async Task StartAsync(string token)
    {
        _client.MessageReceived += OnMessageReceivedAsync;
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    private async Task OnMessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        var content = message.Content.Trim();
        if (!content.StartsWith("!")) return;

        var split = content.Substring(1).Split(' ', 2);

        var cmd = split[0].ToLower();
        var param = split.Length > 1 ? split[1].Trim() : "";

        var currentUser = SessionManager.GetUser(message.Author.Id);

        try
        {
            switch (cmd)
            {
                case "register":
                    {
                        if (currentUser != null)
                        {
                            await message.Channel.SendMessageAsync("❗ You are already logged in. Please logout before registering a new account.");
                            break;
                        }
                        var p = param.Split(' ');
                        if (p.Length != 2)
                        {
                            await message.Channel.SendMessageAsync("❗ Usage: `!register <username> <password>`");
                            break;
                        }

                        await _userService.RegisterAsync(p[0], p[1]);
                        await message.Channel.SendMessageAsync("✅ Registered successfully!");
                        break;
                    }
                case "login":
                    {
                        if (currentUser != null)
                        {
                            await message.Channel.SendMessageAsync("❗ You are already logged in. Please logout before logging in with another account.");
                            break;
                        }
                        var p = param.Split(' ');
                        if (p.Length != 2)
                        {
                            await message.Channel.SendMessageAsync("❗ Usage: `!login <username> <password>`");
                            break;
                        }

                        var user = await _userService.LoginAsync(p[0], p[1]);
                        SessionManager.SetUser(message.Author.Id, user);
                        await message.Channel.SendMessageAsync($"✅ Logged in as **{user.Username}** ({user.UserRole})");
                        break;
                    }
                case "logout":
                    {
                        SessionManager.RemoveUser(message.Author.Id);
                        await message.Channel.SendMessageAsync("✅ Logged out.");
                        break;
                    }
                case "tasks":
                    {
                        if (currentUser == null)
                        {
                            await message.Channel.SendMessageAsync("❗ You must login first.");
                            break;
                        }
                        var tasks = await _taskService.GetAllTasksAsync();
                        if (!tasks.Any())
                        {
                            await message.Channel.SendMessageAsync("📭 No tasks found.");
                            break;
                        }

                        var sb = new StringBuilder("📋 **All Tasks:**\n");
                        foreach (var t in tasks)
                        {
                            var userTag = t.AssignedUser != null ? t.AssignedUser.Username : "Unassigned";
                            sb.AppendLine($"`[{t.Id}]` **{t.Title}** — {t.Status}, due {t.Deadline:yyyy-MM-dd} (User: {userTag})");
                        }
                        await message.Channel.SendMessageAsync(sb.ToString());
                        break;
                    }
                case "createtask":
                    {
                        if (currentUser == null)
                        {
                            await message.Channel.SendMessageAsync("❗ You must login first.");
                            break;
                        }
                        var parts = param.Split('|');
                        if (parts.Length != 3)
                        {
                            await message.Channel.SendMessageAsync("❗ Usage: `!createtask Title | Description | yyyy-MM-dd`");
                            break;
                        }

                        var task = new TaskHolder
                        {
                            Title = parts[0].Trim(),
                            Description = parts[1].Trim(),
                            Deadline = DateTime.Parse(parts[2].Trim()),
                            UserId = currentUser.UserRole == Enums.UserRole.Admin ? (int?)null : currentUser.Id
                        };
                        await _taskService.AddTaskAsync(task);
                        await message.Channel.SendMessageAsync("✅ Task created.");
                        break;
                    }
                case "searchtask":
                    {
                        if (string.IsNullOrWhiteSpace(param))
                        {
                            await message.Channel.SendMessageAsync("❗ Usage: `!searchtask <title>`");
                            break;
                        }
                        var tasks = await _taskService.SearchByTitleAsync(param);
                        if (!tasks.Any())
                        {
                            await message.Channel.SendMessageAsync("🔍 No matching tasks.");
                            break;
                        }
                        var sb = new StringBuilder("🔎 **Search Results:**\n");
                        foreach (var t in tasks)
                        {
                            var userTag = t.AssignedUser != null ? t.AssignedUser.Username : "Unassigned";
                            sb.AppendLine($"`[{t.Id}]` **{t.Title}** — {t.Status}, due {t.Deadline:yyyy-MM-dd} (User: {userTag})");
                        }
                        await message.Channel.SendMessageAsync(sb.ToString());
                        break;
                    }
                case "filterstatus":
                    {
                        if (!Enum.TryParse<Enums.TaskStatus>(param, true, out var status))
                        {
                            await message.Channel.SendMessageAsync("❗ Usage: `!filterstatus <ToDo|InProgress|Done>`");
                            break;
                        }

                        var tasks = await _taskService.FilterByStatusAsync(status);

                        if (tasks.Count == 0)
                        {
                            await message.Channel.SendMessageAsync($"No tasks found with status: {status}");
                            break;
                        }

                        var sb = new StringBuilder();
                        sb.AppendLine($"--- Tasks with status {status} ---");
                        foreach (var task in tasks)
                        {
                            string userPart = task.AssignedUser != null ? $"(User: {task.AssignedUser.Username})" : "(Unassigned)";
                            sb.AppendLine($"{task.Id}. {task.Title} [{task.Status}] Deadline: {task.Deadline:yyyy-MM-dd} {userPart}");
                        }

                        await message.Channel.SendMessageAsync(sb.ToString());
                        break;
                    }
                case "filteruser":
                    {
                        if (!int.TryParse(param, out var uid))
                        {
                            await message.Channel.SendMessageAsync("❗ Usage: `!filteruser <userId>`");
                            break;
                        }

                        var tasks = await _taskService.FilterByUserAsync(uid);

                        if (tasks.Count == 0)
                        {
                            await message.Channel.SendMessageAsync($"No tasks assigned to user with ID: {uid}");
                            break;
                        }

                        var sb = new StringBuilder();
                        sb.AppendLine($"--- Tasks assigned to user ID {uid} ---");
                        foreach (var task in tasks)
                        {
                            string userPart = task.AssignedUser != null ? $"(User: {task.AssignedUser.Username})" : "(Unassigned)";
                            sb.AppendLine($"{task.Id}. {task.Title} [{task.Status}] Deadline: {task.Deadline:yyyy-MM-dd} {userPart}");
                        }

                        await message.Channel.SendMessageAsync(sb.ToString());
                        break;
                    }
                case "filterdeadline":
                    {
                        if (!DateTime.TryParse(param, out var dt))
                        {
                            await message.Channel.SendMessageAsync("❗ Usage: `!filterdeadline <yyyy-MM-dd>`");
                            break;
                        }

                        var tasks = await _taskService.FilterByDeadlineAsync(dt);

                        if (tasks.Count == 0)
                        {
                            await message.Channel.SendMessageAsync($"No tasks with deadline on: {dt:yyyy-MM-dd}");
                            break;
                        }

                        var sb = new StringBuilder();
                        sb.AppendLine($"--- Tasks with deadline {dt:yyyy-MM-dd} ---");
                        foreach (var task in tasks)
                        {
                            string userPart = task.AssignedUser != null ? $"(User: {task.AssignedUser.Username})" : "(Unassigned)";
                            sb.AppendLine($"{task.Id}. {task.Title} [{task.Status}] Deadline: {task.Deadline:yyyy-MM-dd} {userPart}");
                        }

                        await message.Channel.SendMessageAsync(sb.ToString());
                        break;
                    }
                case "deletetask":
                    {
                        if (!int.TryParse(param, out var tid))
                        {
                            await message.Channel.SendMessageAsync("❗ Usage: `!deletetask <taskId>`");
                            break;
                        }
                        await _taskService.DeleteTaskAsync(tid);
                        await message.Channel.SendMessageAsync("🗑️ Task deleted (if existed).");
                        break;
                    }
                case "users":
                    {
                        if (currentUser?.UserRole != Enums.UserRole.Admin)
                        {
                            await message.Channel.SendMessageAsync("❌ Unauthorized.");
                            break;
                        }
                        var users = _userService.GetAllUsers();
                        var sb = new StringBuilder("👥 **Users:**\n");
                        foreach (var u in users)
                            sb.AppendLine($"`[{u.Id}]` **{u.Username}** — {u.UserRole}");
                        await message.Channel.SendMessageAsync(sb.ToString());
                        break;
                    }
                case "help":
                default:
                    await message.Channel.SendMessageAsync(
                        "**Task Bot Commands:**\n" +
                        "`!register <username> <password>`\n" +
                        "`!login <username> <password>`\n" +
                        "`!logout`\n" +
                        "`!tasks`\n" +
                        "`!createtask Title | Description | yyyy-MM-dd`\n" +
                        "`!searchtask <title>`\n" +
                        "`!filterstatus <ToDo|InProgress|Done>`\n" +
                        "`!filteruser <userId>`\n" +
                        "`!filterdeadline <yyyy-MM-dd>`\n" +
                        "`!deletetask <taskId>`\n" +
                        "`!users` (Admin only)\n" +
                        "`!help`");
                    break;
            }
        }
        catch (Exception ex)
        {
            await message.Channel.SendMessageAsync($"❌ Error: {ex.Message}");
        }
    }
}