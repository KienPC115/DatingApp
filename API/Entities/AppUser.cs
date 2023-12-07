using System.ComponentModel.DataAnnotations;
using API.Extentions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

// IdentityUser<int> -> to set the Id using int type. Default is string
public class AppUser : IdentityUser<int>
{
    // The AspNetCore.Identity will take care of this for us (id, userName, Password hash, salt)
    // [Key]
    // public int Id { get; set; }

    // public string UserName { get; set; }

    // public byte[] PasswordHash { get; set; }

    // public byte[] PasswordSalt { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string KnowAs { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow; // initial value

    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    public string Gender { get; set; }

    public string Introduction { get; set; }

    public string LookingFor { get; set; }

    public string Interests { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public List<Photo> Photos { get; set;} = new();

    public List<UserLike> LikedByUsers { get; set; } // who has liked the current user

    public List<UserLike> LikedUsers { get; set; } // who the current user liked

    public List<Message> MessagesSent { get; set; } // represent the message which the current user sent for the other user

    public List<Message> MessagesReceived { get; set; } // represent the message which the current user have been received from the other user

    public ICollection<AppUserRole> UserRoles { get; set; }
}
