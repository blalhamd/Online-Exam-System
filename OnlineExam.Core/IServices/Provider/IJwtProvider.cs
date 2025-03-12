namespace OnlineExam.Core.IServices.Provider
{
    public interface IJwtProvider
    {
        (string token, int expireIn) GenerateToken(AppUser applicationUser, IEnumerable<string> roles, IEnumerable<string> permissions);
    }
}
