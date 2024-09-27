using Microsoft.AspNetCore.SignalR;
using CourseAPI.Dtos; // Make sure to include this if you want to use GetCourseDto
using System;
using System.Threading.Tasks;

namespace CourseAPI.Hubs
{
    public class CourseHub : Hub
    {
        // Method to send course updates to connected clients
        public async Task SendCourseUpdate(GetCourseDto courseDto)
        {
            try
            {
                await Clients.All.SendAsync("ReceiveCourseUpdate", courseDto);
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error sending course update: {ex.Message}");
            }
        }

        // Override for when a client connects
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            // Log or notify about the new connection
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
        }

        // Override for when a client disconnects
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            // Log or notify about the disconnection
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        }

        // Method for clients to join a specific group
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"Client {Context.ConnectionId} joined group: {groupName}");
        }

        // Method for clients to leave a specific group
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine($"Client {Context.ConnectionId} left group: {groupName}");
        }

        // Method to send course updates to a specific group
        public async Task SendCourseUpdateToGroup(string groupName, GetCourseDto courseDto)
        {
            try
            {
                await Clients.Group(groupName).SendAsync("ReceiveCourseUpdate", courseDto);
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error sending course update to group {groupName}: {ex.Message}");
            }
        }
    }
}
