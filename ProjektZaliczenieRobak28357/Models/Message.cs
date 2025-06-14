using System;

namespace AzureChat.Models;

public class Message
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    public User? User { get; set; }
}
