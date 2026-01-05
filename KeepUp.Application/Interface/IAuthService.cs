namespace KeepUp.Application.Interface
{
    public interface IAuthService
    {
        Task<Guid> RegisterAsync(string email, string password, string displayName, DateOnly? dob);
        Task<string> LoginAsync(string email, string password);
    }
}
