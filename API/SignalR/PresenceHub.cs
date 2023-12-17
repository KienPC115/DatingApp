using API.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

// PresenceHub is to display presence of a user in our application
// Inside our Hub we can override a couple of methods.
// -> One of them is when a user connects to this hub.
// -> The other one is when a user disconnects from the hub -> so that we can do something when these events happen
/*
    - This is not going to be an HTTP request that makes this connection to our Hub
    - We use WebSockets (là giao thức liên lạc 2 chiều giữa client với server real-time) - most commonly in all modern browsers
    - We don't have access to HTTP headers when we're not using an HTTP request
    -> vì thế ta phải set-up cách khác để có thể authen -> set-up ở phần IdetityServiceExtensions
*/
[Authorize] // authenticating to our new hub
public class PresenceHub : Hub
{
    // this object is a tracker of presence user who is loggin our application
    // this tracker using an in memory on our server. It's just a single server approach because we're going to
    // be storing it in memory of the server that i have connected to
    // if we have more than one server, then it wouldn't work
    private readonly PresenceTracker _tracker;

    public PresenceHub(PresenceTracker tracker)
    {
        _tracker = tracker;
    }

    public override async Task OnConnectedAsync()
    {
        // check the user is really online with only one ConnectionId
        var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        // Clients object can be used to invoke methods on clients that are connected to this hub
        // anybody else that's connected to this same hub is going to receive the username that have just connected
        // Sẽ gửi message về cho Client-side
        if(isOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername()); // Context is present for our Hub Context - giống như ở Controller - các Controller chứa các route endpoint, it gives us to access to our user claims principle

        // we send currentUsers back to all of the connected clients when somebody connects.
        // alow clients connected to our application to update their list of who is currently online
        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers); // send it for who call this method OnConnectedAsync
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // real offline -> just 0 ConnectionId in tracker pool
        var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

        if(isOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

        await base.OnDisconnectedAsync(exception);
    }
}