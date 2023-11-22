namespace API.Entities;

public class Message
{
    // logic is the same with UserLike entity
    public int Id { get; set; }

    public int SenderId { get; set; }

    public string SenderUsername { get; set; }

    public AppUser Sender { get; set; }

    public int RecipientId { get; set; }

    public string RecipientUsername { get; set; }

    public AppUser Recipient { get; set; }

    public string Content { get; set; }

    public DateTime? DateRead { get; set; }

    public DateTime MessageSent { get; set; } = DateTime.UtcNow; // the message was sent

    // when the the both of those are true will we actually delete the message from the database
    public bool SenderDeleted { get; set; }

    public bool RecipientDeleted { get; set; }
}