using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class MessageHub : Hub
{
    private readonly IHubContext<PresenceHub> _presenceHub;

    // because when a user connects to Message Hub, we're going to want to return the message thread btw them.
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository,
        IMapper mapper, IHubContext<PresenceHub> presenceHub)
    {
        _presenceHub = presenceHub;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    // Why we need Group, Connection Entity

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"];
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        // add group of 2 user to IGroupManager of SignalR-Hub
        // ta phải add ConnnectionId đại diện cho mỗi user vào groupName
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        // add the currently connectionId into group of DB
        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            throw new HubException("You cannot send messages to yourself");

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) throw new HubException("Not found user");

        var message = new Message
        {
            // không cần định nghĩa Id vì đã có đối tượng AppUser -> EF sẽ lấy nó ra và gán vào
            Sender = sender,
            Recipient = recipient,
            // nhưng phải định nghĩa UserName -> vì EF sẽ không quan tâm đến fields khác trừ ID
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);

        var group = await _messageRepository.GetMessageGroup(groupName);

        // check the recipient already have connect with hub -> if yes this user - recipient is on the chat -> we can mark the message as read
        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            // send the notification for user
            var connections = await PresenceTracker.GetConnectionForUser(recipient.UserName);
            if (connections != null)
            {
                // we can access other hubs by injecting them into the hub that we're working on
                // vì những thằng này đang connect ở presence Hub, còn message Hub thì không thế nên ta inject và sử dụng nó để gửi thông báo
                // Clients.Clients(list of connectionId) -> we use this method to send message to all of the client connected by connectionID
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                new { username = sender.UserName, knowAs = sender.KnowAs });
            }
        }

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
        {
            // all user in this group will be recieved the new message of the sender
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    private string GetGroupName(string caller, string other)
    {
        // sort by alphabet order - sắp trước đứng trước
        // chỉ là đặt tên cho nhóm - 2 người nhắn tin với nhau - private message
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        // add a connectionId to group (if not it will be add into DB)
        var group = await _messageRepository.GetMessageGroup(groupName);

        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        if (group == null)
        {
            group = new Group(groupName);
            _messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        if (await _messageRepository.SaveAllAsync()) {
            return group;
        }

        throw new HubException("Fail to add to group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        // remove connectionId out database
        var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _messageRepository.RemoveConnection(connection);

        if(await _messageRepository.SaveAllAsync()) return group;

        throw new HubException("Fail to remove from group");
    }
}