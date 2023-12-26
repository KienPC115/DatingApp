using API.DTOs;
using API.Entities;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MemberDto> GetMemberAsync(string username, bool isCurrentUser)
    {
        var query = _context.Users
        .Where(x => x.UserName == username)
        // AutoMapper come with variable extensions which allow us to project into something
        // Use this it help reduce query all properties, help we call some prop that neccessary and map with a given Dto
        // take care for us about Eager Loading(MemberDto has a field list of PhotoDto)
        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)// this arguement is the config in Profile,
        // .SingleOrDefaultAsync();
        .AsQueryable();

        if (isCurrentUser)
        {
            query = query.IgnoreQueryFilters(); // -> to ignore the query be settup in DataContext and get all records.
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var query = _context.Users.AsQueryable();

        query = query.Where(u => u.UserName != userParams.CurrentUsername);
        query = query.Where(u => u.Gender == userParams.Gender);

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActive)
        };

        return await PagedList<MemberDto>.CreateAsync(
            query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
            userParams.PageNumber,
            userParams.PageSize);
    }

    public async Task<IEnumerable<AppUser>> GetUserAsync()
    {
        // Eager Loading -> will cause the error Object Cycle -> AppUser has list of Photos, Photos has a AppUser 
        //-> so that will have a call cycle -> to solve this problem we create Dto and adding AutoMapper
        return await _context.Users
        .Include(p => p.Photos)
        .ToListAsync();
        // return a list of user with each user have a fetched photo.
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByPhotoId(int photoId)
    {
        return await _context.Users
            .Include(u => u.Photos)
            .IgnoreQueryFilters()
            .Where(u => u.Photos.Any(x => x.Id == photoId))
            .FirstOrDefaultAsync();
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        // will same problem with GetUserAsync()
        return await _context.Users
        .Include(p => p.Photos)
        .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<string> GetUserGender(string username)
    {
        return await _context.Users.Where(x => x.UserName == username)
            .Select(x => x.Gender)
            .FirstOrDefaultAsync();
    }

    public void Update(AppUser user)
    {
        // informing the Entity Framework Tracker that an entity has been updated
        _context.Entry(user).State = EntityState.Modified;
    }
}