namespace API.Helper;

public class CloudinarySettings
{
    // dotnet add package CloudinaryDotNet to use the third-party Cloudinary to store the photos
    // in the Cloudinary we will have 3 following properties to set up and make it security -> only our services can upload photo to Cloudinary

    public string CloudName { get; set; }

    public string ApiKey { get; set; }
    
    public string ApiSecret { get; set; }

}