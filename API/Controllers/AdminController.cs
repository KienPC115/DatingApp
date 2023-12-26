using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _uow;
    private readonly IPhotoService _photoService;

    public AdminController(UserManager<AppUser> userManager, IUnitOfWork uow, IPhotoService photoService)
    {
        _userManager = userManager;
        _uow = uow;
        _photoService = photoService;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.UserName)
            .Select(u => new
            {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

        var selectedRoles = roles.Split(",").ToArray(); // new roles of user

        // this is method of userManager is quit useful to find out the user with username we passed
        var user = await _userManager.FindByNameAsync(username);

        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);

        // add new Roles
        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        // remove the unneeded roles
        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove form roles");

        return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult<IEnumerable<PhotoForApprovalDto>>> GetPhotosForModeration()
    {
        var photos = await _uow.PhotoRepository.GetUnapprovedPhotos();

        return Ok(photos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await _uow.PhotoRepository.GetPhotoById(photoId);

        if (photo == null) return NotFound("Could not find photo");

        if (photo.IsApproved) return BadRequest("This photo is already approved");

        photo.IsApproved = true;

        var user = await _uow.UserRepository.GetUserByPhotoId(photoId);
        if (!user.Photos.Any(p => p.IsMain)) photo.IsMain = true; // if not any photo are main, then set the photo to main when approving

        if (await _uow.Complete()) return Ok();

        return BadRequest("Fail to approve this photo");
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await _uow.PhotoRepository.GetPhotoById(photoId);

        if (photo == null) return NotFound();

        if (photo.IsApproved) return BadRequest("This photo is already approved");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        _uow.PhotoRepository.RemovePhoto(photo);

        if (await _uow.Complete()) return Ok();

        return BadRequest("Fail to reject this photo");
    }
}