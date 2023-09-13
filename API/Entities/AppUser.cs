using System.ComponentModel.DataAnnotations;
using SQLitePCL;

namespace API.Entities;

public class AppUser
{
    [Key]
    public int Id { get; set; }

    public string UserName { get; set; }

    public byte[] PasswordHash { get; set; }

    public byte[] PasswordSalt { get; set; }
}