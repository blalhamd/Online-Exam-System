namespace OnlineExam.Core.IServices
{
    public interface IAuthenticationService
    {
        Task LoginAsync(LoginRequest request);
        Task LogoutAsync(string userId);
    }
}
