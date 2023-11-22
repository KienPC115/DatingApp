namespace API.Helper;

public class MessageParams : PaginationParams
{
    public string Username { get; set; } // be the currently logged in username

    public string Container { get; set; } = "Unread";
}