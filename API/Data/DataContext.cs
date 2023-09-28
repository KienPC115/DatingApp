using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

// this class can present the Database with the table, or this is the session work that we access database 
public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; }

}