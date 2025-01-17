using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Shop_Classix.Helper
{
    public class CustomUserIdProvider:IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            //sử dụng email hoặc id từ claim
            return connection.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }
    }
}
