using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatService;
using System.Linq;
using System;
using ChatService.Models;
using AutoMapper;
using ChatService.DTOs;

namespace SignalRChat.Hubs
{
  public class ChatHub : Hub
  {
    private readonly string _botUser;
    //private readonly IDictionary<string, UserConnection> _connections;

    public readonly static List<User> _Connections = new List<User>();
    private readonly static Dictionary<string, string> _ConnectionsMap = new Dictionary<string, string>();
    private readonly IDictionary<string, UserConnection> _connections;

    private readonly ChatDBContext _chatdbcontext;
    // private readonly IMapper _mapper;
    //public ChatHub(IDictionary<string, UserConnection> connections)
    public ChatHub(ChatDBContext chatdbcontext, IDictionary<string, UserConnection> connections)
    {
      _botUser = "MyChat Bot";

      _chatdbcontext = chatdbcontext;
      _connections = connections;

    }

    //public override Task OnDisconnectedAsync(Exception exception)
    //{
    //  if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
    //  {
    //    _connections.Remove(Context.ConnectionId);
    //    Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has left");
    //    SendUsersConnected(userConnection.Room);
    //  }

    //  return base.OnDisconnectedAsync(exception);
    //}

    public async Task JoinRoom(UserConnection userConnection)
    {
      try
      {
        await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

        _connections[Context.ConnectionId] = userConnection;

        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has joined {userConnection.Room}");

        await SendUsersConnected(userConnection.Room);

     
      }
      catch (Exception ex)
      {
        await Clients.Caller.SendAsync("onError", "You failed to join the chat room!" + ex.Message);
      }
    }

    public Task SendUsersConnected(string room)
    {
      var users = _connections.Values
          .Where(c => c.Room == room)
          .Select(c => c.User);

      return Clients.Group(room).SendAsync("UsersInRoom", users);
    }

    public async Task Leave(string roomName)
    {
      await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }

    public async Task SendMessage(string message)
    {
      if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
      {
        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", userConnection.User, message);
      }
    }

    //public Task SendUsersConnected(string room)
    //{
    //  //var users = _connections.Values
    //  //    .Where(c => c.Room == room)
    //  //    .Select(c => c.User);

    //  //return Clients.Group(room).SendAsync("UsersInRoom", users);
    //}
  }
}
