using MyNotes.API.Models.DtoModels;
using MyNotes.API.Models.ErrorResponse;

namespace MyNotes.API.Repositories;

public interface IUsersRepository
{
    public Task<ResultResponse<UserResponse>?> GetUserInformation(UserCredentials userCredentials,
        CancellationToken token = default);

    public Task<ResultResponse<bool>> CreateUser(UserUpload userUpload, CancellationToken token = default);
    public Task<ResultResponse<bool>> UpdateUser(int userId, UserUpload userUpload, CancellationToken token = default);

    public Task<ResultResponse<bool>> DeleteUser(int userId, CancellationToken token = default);
    // TODO : Research and add a apikey reset method
}