namespace API.Entities;

public class UserLike
{
    // hint to know this:
    // sourceUser is the person has liked the TargetUser
    // base on this logic
    // When we want get the list user have been liked by logged-in user
    //-> get the UserLike have sourceUserid = current userId -> the targetUser will be the person who you has liked
    // with the list user liked the current user is the same logic.

    public AppUser SourceUser { get; set; }

    public int SourceUserId  { get; set; }

    public AppUser TargetUser { get; set; }

    public int TargetUserId { get; set; }
}