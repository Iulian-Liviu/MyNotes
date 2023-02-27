using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyNotes.API.Database;
using MyNotes.API.Helpers;
using MyNotes.API.Models;
using MyNotes.API.Models.DtoModels;
using MyNotes.API.Models.ErrorResponse;


namespace MyNotes.API.Repositories;

public class UsersRepository : IUsersRepository {
    private readonly MyNotesDbContext _context;
    private readonly IMapper _mapper;
    private bool disposedValue;

    public UsersRepository(IMapper mapper, MyNotesDbContext context) {
        _mapper = mapper;
        _context = context;
    }

    public async Task<ResultResponse<bool>> CreateUser(UserUpload userUpload, CancellationToken token) {
        var userExists =
            await _context.Users.Where(u => u.Email == userUpload.Email || u.UserName == userUpload.UserName
                                                                        || u.Name == userUpload.Name)
                .FirstOrDefaultAsync(token);
        if (userExists != null) {
            if (userExists.Email == userUpload.Email) {
                return new ResultResponse<bool> {
                    StatusCode = 400,
                    StatusType = StatusType.Bad,
                    DetailedMessage = "The email address you entered is already associated with an existing account. Please use a different email address or log in to your existing account."
                };
            }
            else if (userExists.UserName == userUpload.UserName) {
                return new ResultResponse<bool> {
                    StatusCode = 400,
                    StatusType = StatusType.Bad,
                    DetailedMessage = "The username provided is already taken, please chose another username"
                };
            }
            // TODO Should the "Name" be unique?
            /*            else if (userExists.Name == userUpload.Name) {
                            return new ResultResponse<bool> {
                                StatusCode = 400,
                                StatusType = StatusType.Bad,
                                DetailedMessage = ""
                            };
                        }*/
        }

        var user = new User {
            Email = userUpload.Email,
            Name = userUpload.Name,
            UserName = userUpload.UserName,
            Password = HelpersMethods.HasHPassword(userUpload.Password ?? "password")
        };
        var addedUser = await _context.Users.AddAsync(user, token);
        await _context.SaveChangesAsync(token);
        var apiKey = await _context.Keys.AddAsync(new ApiKey {
            Key = Guid.NewGuid().ToString("N"),
            User = addedUser.Entity,
            UserId = addedUser.Entity.UserId
        }, token);
        await _context.SaveChangesAsync(token);
        return new ResultResponse<bool> {
            StatusCode = 200,
            StatusType = StatusType.Success
        };
    }

    public async Task<ResultResponse<bool>> DeleteUser(int userId, CancellationToken token) {
        var userExists = await _context.Users.FindAsync(userId);
        if (userExists == null)
            return new ResultResponse<bool> {
                StatusCode = 404,
                StatusType = StatusType.NotFound
            };
        ;
        _context.Users.Remove(userExists);
        await _context.SaveChangesAsync(token);
        return new ResultResponse<bool> {
            StatusCode = 200,
            StatusType = StatusType.Success
        };
    }

    public async Task<ResultResponse<UserResponse>> GetUserInformation(UserCredentials userCredentials,
        CancellationToken token) {
        var foundUser = await _context.Users.Where(u =>
                u.Email == userCredentials.Email)
            .FirstOrDefaultAsync(token);
        if (foundUser == null)
            return new ResultResponse<UserResponse> {
                StatusCode = 404,
                StatusType = StatusType.NotFound
            };

        return HelpersMethods.IsPasswordMatch(foundUser!.Password!, userCredentials!.Password!)
            ? new ResultResponse<UserResponse> {
                Data = _mapper.Map<UserResponse>(foundUser),
                StatusType = StatusType.Success,
                StatusCode = 200
            }
            : new ResultResponse<UserResponse> {
                StatusCode = 201,
                StatusType = StatusType.Bad
            };
    }

    public async Task<ResultResponse<bool>> UpdateUser(int userId, UserUpload userUpload, CancellationToken token) {
        var userExists = await _context.Users.FindAsync(userId);
        if (userExists == null)
            return new ResultResponse<bool> {
                StatusCode = 404,
                StatusType = StatusType.NotFound
            };

        userExists.UserName = string.IsNullOrEmpty(userUpload.UserName) ? userExists.UserName : userUpload.UserName;
        userExists.Email = string.IsNullOrEmpty(userUpload.Email) ? userExists.Email : userUpload.Email;
        userExists.Name = string.IsNullOrEmpty(userUpload.Name) ? userExists.Name : userUpload.Name;
        userExists.Password = string.IsNullOrEmpty(userUpload.Password) ? userExists.Password : HelpersMethods.HasHPassword(userUpload.Password);

        _context.Users.Update(userExists);
        await _context.SaveChangesAsync(token);

        return new ResultResponse<bool> {
            StatusCode = 200,
            StatusType = StatusType.Success
        };
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                // TODO: dispose managed state (managed objects)
                _context.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~UsersRepository()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}