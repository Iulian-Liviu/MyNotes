using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyNotes.API.Database;
using MyNotes.API.Helpers;
using MyNotes.API.Models;
using MyNotes.API.Models.DtoModels;
using MyNotes.API.Models.ErrorResponse;

namespace MyNotes.API.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly MyNotesDbContext _context;
    private readonly IMapper _mapper;

    public UsersRepository(IMapper mapper, MyNotesDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<ResultResponse<bool>> CreateUser(UserUpload userUpload, CancellationToken token)
    {
        var userExists =
            await _context.Users.Where(u => u.Email == userUpload.Email || u.UserName == userUpload.UserName
                                                                        || u.Name == userUpload.Name)
                .FirstOrDefaultAsync(token);
        if (userExists != null)
            return new ResultResponse<bool>
            {
                StatusCode = 404,
                StatusType = StatusType.NotFound
            };

        var user = new User
        {
            Email = userUpload.Email,
            Name = userUpload.Name,
            UserName = userUpload.UserName,
            Password = HelpersMethods.HasHPassword(userUpload.Password ?? "password")
        };
        var addedUser = await _context.Users.AddAsync(user, token);
        await _context.SaveChangesAsync(token);
        var apiKey = await _context.Keys.AddAsync(new ApiKey
        {
            Key = Guid.NewGuid().ToString("N"),
            User = addedUser.Entity,
            UserId = addedUser.Entity.UserId
        }, token);
        await _context.SaveChangesAsync(token);
        return new ResultResponse<bool>
        {
            StatusCode = 200,
            StatusType = StatusType.Success
        };
    }

    public async Task<ResultResponse<bool>> DeleteUser(int userId, CancellationToken token)
    {
        var userExists = await _context.Users.FindAsync(userId);
        if (userExists == null)
            return new ResultResponse<bool>
            {
                StatusCode = 404,
                StatusType = StatusType.NotFound
            };
        ;
        _context.Users.Remove(userExists);
        await _context.SaveChangesAsync(token);
        return new ResultResponse<bool>
        {
            StatusCode = 200,
            StatusType = StatusType.Success
        };
    }

    public async Task<ResultResponse<UserResponse>?> GetUserInformation(UserCredentials userCredentials,
        CancellationToken token)
    {
        var foundUser = await _context.Users.Where(u =>
                u.Email == userCredentials.Email)
            .FirstOrDefaultAsync(token);
        return HelpersMethods.IsPasswordMatch(foundUser!.Password!, userCredentials!.Password!)
            ? new ResultResponse<UserResponse>
            {
                Data = _mapper.Map<UserResponse>(foundUser),
                StatusType = StatusType.Success,
                StatusCode = 200
            }
            : null;
    }

    public async Task<ResultResponse<bool>> UpdateUser(int userId, UserUpload userUpload, CancellationToken token)
    {
        var userExists = await _context.Users.FindAsync(userId);
        if (userExists == null)
            return new ResultResponse<bool>
            {
                StatusCode = 404,
                StatusType = StatusType.NotFound
            };

        userExists.UserName = userUpload.UserName;
        userExists.Email = userUpload.Email;
        userExists.Name = userUpload.Name;
        userExists.Password = HelpersMethods.HasHPassword(userUpload.Password!);
        _context.Users.Update(userExists);
        await _context.SaveChangesAsync(token);

        return new ResultResponse<bool>
        {
            StatusCode = 200,
            StatusType = StatusType.Success
        };
    }
}