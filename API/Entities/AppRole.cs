using Microsoft.AspNetCore.Identity;

namespace API.Entities;

// it have many to many relationship btw our app user and our app role
// because each role can have many users inside a particular role and each user can be a member of many roles
public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}