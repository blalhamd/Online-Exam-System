namespace OnlineExam.Core.ViewModels.Auth.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public int ExpireIn { get; set; }
    }
}
