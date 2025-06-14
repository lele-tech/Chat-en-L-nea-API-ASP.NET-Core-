using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

public class ChatHub : Hub
{
    // Método para unirse a un grupo
    public Task JoinGroup(string groupName)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    // Método para salir de un grupo
    public Task LeaveGroup(string groupName)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    // Método para enviar un mensaje a un grupo
    public async Task SendMessageToGroup(string groupName, string user, string message, string time)
    {
        await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message, time);
    }
}
