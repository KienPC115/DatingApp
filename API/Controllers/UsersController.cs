using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
// /api/users
[Authorize] // this attribute in here is the controller-level -> will be set all the endpoint with this attribute -> [AllowAnonymus] can be use at same controller
// nhưng ngược lại thì không [AllowAnonymus] at controller-level -> will ignore the [Authorize]
public class UsersController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUnitOfWork uow, IMapper mapper,
        IPhotoService photoService)
    {
        _uow = uow;
        _mapper = mapper;
        _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        var gender = await _uow.UserRepository.GetUserGender(User.GetUsername());
        userParams.CurrentUsername = User.GetUsername();

        if (string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = gender == "male" ? "female" : "male";
        }

        // get a PagedList it contain all information about pagination
        var users = await _uow.UserRepository.GetMembersAsync(userParams);

        // set up the data in headers of response
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }

    // /api/users/2
    [HttpGet("{username}")] //Authentication our endpoint by the token 
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var isCurrentUser = User.GetUsername() == username;
        return await _uow.UserRepository.GetMemberAsync(username, isCurrentUser);
    }

    [HttpPut] // we dont need to add username in parameter root, we can get it from the token
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        // the User of System.Security.Claims.ClaimsPrincipal
        // it contains all the claims of token from client
        var username = User.GetUsername(); // this is extension method of User claims.
        var user = await _uow.UserRepository.GetUserByUsernameAsync(username);

        if (user == null) return NotFound();

        // this will be map memberUpdateDto to user get from the DB
        // effectively updating all of the properties that we pass through in that memberUpdateDto 
        //into and overwriting the properties in that user.
        _mapper.Map(memberUpdateDto, user);

        if (await _uow.Complete()) return NoContent();

        // if we map the properties the same with properties had stored in DB
        // EF tracker will not consider it is change and Failed to update.
        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        // flow of upload photo in Cloud
        // 1. client upload img with the token
        // 2. server add this img in cloud using Cloudinary service with api key(api secret)
        // 3. Saved photo will send for server the Url(where img locate) and publicId of img in Cloud
        // 4. Server save it to DataBase via Photo Entity

        // Here we need get a AppUser because this func is add  the photo for AppUser.Photo and update it to DataBase.
        var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();

        // result - ImageUploadResult (it contain the Url where the img located, and PublicId of this img in the Cloud)
        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        // create a Photo Entity
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId,
            IsApproved = false
        };

        // because AppUser have Eagerloading with photo that why we can use this.
        // if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        // create new respone from the Server need to return a 201 created
        // should give them is the exact location of the new thing that's been created on server
        if (await _uow.Complete())
        {
            // Map method have 2 way: 1-Have a Dto _map.Map(entity, dto). 2-Not have a Dto _map.Map<Dto>(entity)
            // Action same with the endpoint of app
            return CreatedAtAction(nameof(GetUser),
                new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            // => return 201 created response with location details about where to find the newly created resource
            // ex: we create the new photo of user => where to find(can see) the newly created photo is in action GetUser() with the username
        }

        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return NotFound();

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("This is already your main photo");

        if(!photo.IsApproved) return BadRequest("This photo is waiting approval, you can not set it to main");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if (currentMain != null) currentMain.IsMain = false;

        photo.IsMain = true;

        if (await _uow.Complete()) return NoContent();

        return BadRequest("Problem setting the main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        // delete here mean delete in the database, and delete in the cloudinary server
        // var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        var photo = await _uow.PhotoRepository.GetPhotoById(photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("You can not delete your main photo");

        // here have some photo does not have public id
        // if they dont have, it does not have to remove in the cloudinary server
        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        _uow.PhotoRepository.RemovePhoto(photo);

        if (await _uow.Complete()) return Ok();

        return BadRequest("Problem deleting photo");
    }

}