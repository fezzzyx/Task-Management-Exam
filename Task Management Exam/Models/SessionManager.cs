using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Management_Exam.Models;

public class SessionManager
{
    private static Dictionary<ulong, User> _sessions = new Dictionary<ulong, User>();

    public static void SetUser(ulong discordId, User user)
    {
        _sessions[discordId] = user;
    }

    public static User? GetUser(ulong discordId)
    {
        if (_sessions.TryGetValue(discordId, out var user))
        {
            return user;
        }
        else
        {
            return null;
        }
    }

    public static void RemoveUser(ulong discordId)
    {
        _sessions.Remove(discordId);
    }
}
