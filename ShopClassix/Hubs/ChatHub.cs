using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shop_Classix.Models;
using Shop_Classix.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private readonly DataContext _context;

    public ChatHub(DataContext context)
    {
        _context = context;
    }

    //// Dictionary to store admin connections (by ConnectionId)
    //private static Dictionary<string, string> _adminConnections = new Dictionary<string, string>();

    //// When an Admin connects
    //public override async Task OnConnectedAsync()
    //{
    //    string userName = Context.User.Identity.Name;

    //    if (Context.User.IsInRole("Admin"))
    //    {
    //        _adminConnections[Context.ConnectionId] = userName; // Save ConnectionId for Admin
    //        Console.WriteLine($"Admin {userName} connected");
    //    }

    //    await base.OnConnectedAsync();
    //}

    //// When an Admin disconnects
    //public override async Task OnDisconnectedAsync(Exception? exception)
    //{
    //    if (_adminConnections.ContainsKey(Context.ConnectionId))
    //    {
    //        var userName = _adminConnections[Context.ConnectionId];
    //        _adminConnections.Remove(Context.ConnectionId);  // Remove admin from active connections
    //        Console.WriteLine($"Admin {userName} disconnected");
    //    }

    //    await base.OnDisconnectedAsync(exception);
    //}

    //// Send message from Admin to User
    //public async Task SendMessageToUser(string userId, string message)
    //{
    //    if (Context.User.IsInRole("Admin"))
    //    {
    //        // Get the AccountId dynamically based on the logged-in admin user
    //        var adminAccount = await _context.customers
    //            .Where(c => c.Email == Context.User.Identity.Name)
    //            .FirstOrDefaultAsync();

    //        if (adminAccount != null)
    //        {
    //            var userMessage = new ChatUsersModel
    //            {
    //                sender = Context.User.Identity.Name,
    //                receiver = userId,
    //                MessageContent = message,
    //                AccountId = adminAccount.Id, // Dynamic AccountId for the admin
    //                CreateAt = DateTime.Now
    //            };

    //            try
    //            {
    //                _context.chatUsers.Add(userMessage);
    //                await _context.SaveChangesAsync();

    //                // Send message to User (ensure user exists in the system)
    //                await Clients.User(userId).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
    //            }
    //            catch (Exception ex)
    //            {
    //                Console.WriteLine("Error saving message: " + ex.Message);
    //            }
    //        }
    //    }
    //}

    //// Send message from User to Admin
    //// Send message from User to Admin
    //public async Task SendMessageToAdmin(string message)
    //{
    //    if (Context.User.IsInRole("User"))
    //    {
    //        // Check if any admin is connected
    //        var adminConnectionId = _adminConnections.Values.FirstOrDefault();

    //        if (string.IsNullOrEmpty(adminConnectionId))
    //        {
    //            Console.WriteLine("No active admin connections.");
    //            // Optionally, notify the user if no admin is available
    //            await Clients.Caller.SendAsync("NoAdminAvailable", "Currently, no admin is available to chat.");
    //        }
    //        else
    //        {
    //            var userAccount = await _context.customers
    //                .Where(c => c.Email == Context.User.Identity.Name)
    //                .FirstOrDefaultAsync();

    //            if (userAccount != null)
    //            {
    //                var userMessage = new ChatUsersModel
    //                {
    //                    sender = Context.User.Identity.Name,
    //                    receiver = adminConnectionId, // Send message to the connected admin
    //                    MessageContent = message,
    //                    AccountId = userAccount.Id, // Dynamic AccountId for the user
    //                    CreateAt = DateTime.Now
    //                };

    //                try
    //                {
    //                    _context.chatUsers.Add(userMessage);
    //                    await _context.SaveChangesAsync();

    //                    // Send message to Admin
    //                    await Clients.Client(adminConnectionId).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
    //                }
    //                catch (Exception ex)
    //                {
    //                    Console.WriteLine("Error saving message: " + ex.Message);
    //                }
    //            }
    //        }
    // }
    //}


    // To send a message to a specific user
    public async Task SendMessageToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
    }

    // To send a message to the admin
    public async Task SendMessageToAdmin(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
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
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
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
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing user from group: {ex.Message}");
        }

        await base.OnDisconnectedAsync(exception);
    }
}