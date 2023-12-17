namespace API.SignalR;

public class PresenceTracker
{
    // Dictionary to store the key-value: key is represent the username, value is a list of connection IDs for 
    //particular user because the user can logging in a different device, but they'll be given a different connection ID
    //if they would have 2 items inside this list, the 2 diff connection IDs would be given, and when they lo out of one, they still connection
    private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

    public Task<bool> UserConnected(string username, string connectionId)
    {
        bool isOnline = false;
        // because dictionary is not a safe thread object - if we had mutiple concurrent users trying to access this dictionary at same time it could run out the issue
        // we use the lock the slove this problem
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                // already have connection -> add more
                OnlineUsers[username].Add(connectionId);
            }
            else
            {
                // real online of user
                OnlineUsers.Add(username, new List<string> { connectionId });
                isOnline = true;
            }
        }

        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(string username, string connectionId) {
        bool isOffline = false;

        lock(OnlineUsers) {
            if(!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

            OnlineUsers[username].Remove(connectionId);

            if(OnlineUsers[username].Count == 0) {
                OnlineUsers.Remove(username);
                isOffline = true;   
            }
        }

        return Task.FromResult(isOffline);
    }

    public Task<string[]> GetOnlineUsers() {
        string[] onlineUsers;

        lock(OnlineUsers) {
            onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
        }

        return Task.FromResult(onlineUsers);
    }

    // Get all the connectionId of user -> because when we want notify the message -> we should do it for all Connection
    public static Task<List<string>> GetConnectionForUser(string username) {
        List<string> connectionIds;

        lock(OnlineUsers) {
            // get all the values by key -> using method GetValueOrDefault()
            connectionIds = OnlineUsers.GetValueOrDefault(username);
        }

        return Task.FromResult(connectionIds);
    }
}