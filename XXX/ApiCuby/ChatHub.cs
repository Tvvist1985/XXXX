using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ApiCuby
{
    public class ChatHub : Hub
    {        
        //Method: транслятор сообщений принимает и отправляет сообщения      
        public async Task Broadcast(string message, string to) 
        {           
            if (Context.UserIdentifier is string userName) // получение текущего пользователя, который отправил сообщение            
                await Clients.Users(to, userName).SendAsync("Receive", message, userName, to); //Отправляю сообщение себе и аппоненту                      
        }

        //Method: вход пользователя
        public override async Task OnConnectedAsync() => await base.OnConnectedAsync();   

        //Method: выход пользователя
        public override async Task OnDisconnectedAsync(Exception? exception) => await base.OnDisconnectedAsync(exception);               
    }
    
    public class CustomUserIdProvider : IUserIdProvider
    {
        //Method: получение id
        public virtual string? GetUserId(HubConnectionContext connection) =>
            connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;          
    }
}
