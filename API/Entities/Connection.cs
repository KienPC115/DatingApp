using System.Text.Json.Serialization;

namespace API.Entities;

public class Connection
{
    // give this an empty ctor for entity framework to it creates the schema for our database, because if we have a ctor parameter, it not expecting to be able to pass the parameter
    public Connection()
    {
    }

    public Connection(string connectionId, string username)
    {
        ConnectionId = connectionId;
        Username = username;
    }

    public string ConnectionId { get; set; }

    public string Username { get; set; }

    public string GroupName { get; set; }

    [JsonIgnore]
    public Group Group { get; set; }
}