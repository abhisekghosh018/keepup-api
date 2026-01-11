using KeepUp.Application.Common;
using KeepUp.Application.DTOs;

namespace KeepUp.Application.Interface
{
    public interface IAuthService
    {
        Task<Result<string>> RegisterAsync(string email, string password, string displayName, DateOnly? dob);
        Task<Result<string>> LoginAsync(string email, string password);
        Task<Result<GetUsersRequest>> GetUserByEmailAsync(string email);
        Task<Result<List<GetUsersRequest>>> GetAllUsers();
    }
}
