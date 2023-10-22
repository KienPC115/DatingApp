using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helper;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    // this method is going to get a list of users liked the currently logged user 
    // or the list of user that user logged has been like. It is base on the predicate
    public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var users = _context.Users.OrderBy(u => u.UserName).AsQueryable(); // get a list of our users in the db ordered by their username
        var likes = _context.Likes.AsQueryable();

        if (likesParams.Predicate == "liked")
        { // it gets the list of users who have been liked by the logged-in user.
            likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
            // we select the users that are inside list likes. Here is like.TargetUser return AppUser as well -> that why we can use the code following
            users = likes.Select(like => like.TargetUser); // filter the user -> return the list of user have the same targetUser
        }

        if (likesParams.Predicate == "likedBy")
        { // This gets the list of users who have liked the logged-in user.
            likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
            users = likes.Select(like => like.SourceUser);
        }

        var likedUsers =  users.Select(user => new LikeDto
        {
            UserName = user.UserName,
            KnowAs = user.KnowAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
            City = user.City,
            Id = user.Id
        });

        return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        return await _context.Users
            .Include(x=> x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}