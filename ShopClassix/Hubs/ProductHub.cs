using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ProductHub : Hub
{
    public async Task UpdateProductCount(int count)
    {
        await Clients.All.SendAsync("ReceiveProductCount", count);
    }
}
