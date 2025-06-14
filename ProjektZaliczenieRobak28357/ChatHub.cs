using AzureChat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AzureChat;

[Authorize]
public class ChatHub : Hub
{
    private readonly AppDbContext _context;

    public ChatHub(AppDbContext context)
    {
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        var messages = await _context.Messages
            .Include(m => m.User)
            .OrderByDescending(m => m.Timestamp)
            .Take(20)
            .ToListAsync();

        messages.Reverse(); // najstarsze pierwsze

        foreach (var msg in messages)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", FormatMessage(msg.User!.Username, msg.Text, msg.Timestamp));
        }

        await base.OnConnectedAsync();
    }

    public async Task SendMessage(string message)
    {
        var username = Context.User?.Identity?.Name ?? "Anonim";
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return;

        var msg = new Message
        {
            Text = message,
            Timestamp = DateTime.UtcNow,
            UserId = user.Id
        };

        _context.Messages.Add(msg);
        await _context.SaveChangesAsync();

        await Clients.All.SendAsync("ReceiveMessage", FormatMessage(user.Username, message, msg.Timestamp));
    }

    private string FormatMessage(string user, string message, DateTime timestamp)
    {
        return $"[{timestamp:HH:mm}] {user}: {message}";
    }
}
