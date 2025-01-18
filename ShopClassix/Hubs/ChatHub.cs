using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Repository;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private readonly DataContext _context;
    private readonly IUserIdProvider _userIdProvider;

    public ChatHub(DataContext context,IUserIdProvider userIdProvider)
    {
        _context = context;
        _userIdProvider=userIdProvider;

    }

    //gửi tin nhắn cho user
    public async Task SendMessageToUser(string email, string message)
    {
        var userRole = Context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userRole) || userRole != "Admin")
        {
            throw new HubException("You do not have permission to send messages.");
        }

        // Proceed to send the message
        await Clients.User(email).SendAsync("ReceiveMessage", Context.User.Identity.Name, message, DateTime.Now.ToString());
    }



    // Send message to the admin
    public async Task SendMessageToAdmin(string message)
    {
        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Add timestamp
        await Clients.All.SendAsync("ReceiveMessage", Context.User.Identity.Name, message, timeStamp);
    }


    // Handle user connection
    public override async Task OnConnectedAsync()
    {
        string userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            Console.WriteLine("User ID is null or empty on connect.");
            return;
        }

        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId); // Add user to group
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding user to group: {ex.Message}");
        }

        await base.OnConnectedAsync();
    }

    // Handle user disconnection
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId))
        {
            Console.WriteLine("User ID is null or empty on disconnect.");
            return;
        }

        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId); // Remove user from group
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing user from group: {ex.Message}");
        }

        await base.OnDisconnectedAsync(exception);
    }


}