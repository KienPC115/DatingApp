using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Photos")]
public class Photo
{
    public int Id { get; set; }

    public string Url { get; set; }

    public bool IsMain { get; set; }

    // when we do get a image upload result back from cloudinary, this is going to give us that publicID and store it to DB
    public string PublicId { get; set; }

    public int AppUserId { get; set; }

    public AppUser AppUser { get; set; }
}
