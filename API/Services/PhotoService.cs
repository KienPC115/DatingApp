using API.Helper;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;

public class PhotoService : IPhotoService
{
    // this obj is represent for Cloudinary Server. We want to use this service
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> config)
    {
        // create a account by ctor
        var acc = new Account
        (
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        // set up an account(sign up) to the Cloudinary thirdparty Service.
        _cloudinary = new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        // that is a rs when we upload img to Cloudinary
        var uploadResult = new ImageUploadResult();

        if(file.Length > 0) 
        {
            using var stream = file.OpenReadStream();   
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                // this help us transform the given image to square,
                // and crop() -> help to cut img if it rectangle,
                // the Gravitty() can specify that we want this to be gravitating towards the face in the img
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "da-net7"
            };

            // i have tip to remember the thirdparty service.
            // just think this service is the special object of our app
            // this object has some method to push/operation the infomation(params) here into there server.
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        return await _cloudinary.DestroyAsync(deleteParams);
    }
}