using System;
using Discord;
using Discord.WebSocket;
using Task_Management_Exam.Data;
using Task_Management_Exam.Models;
using Task_Management_Exam.Services;

var config = new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
};
var client = new DiscordSocketClient(config);
var dbContext = new AppDbContext();
var userService = new UserService(dbContext);
var taskService = new TaskService(dbContext);

client.Log += msg => {
    Console.WriteLine(msg);
    return Task.CompletedTask;
};

var bot = new Bot(client, userService, taskService);

await bot.StartAsync("MTM4OTY2MzA5NDQ2NDA1MzM0OQ.GpR9vx.noMVPK14PXXmW0hPRdrmT7HYzOoTMhGEOjdBWA");

await Task.Delay(-1);