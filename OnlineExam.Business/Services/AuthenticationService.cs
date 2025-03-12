namespace OnlineExam.Business.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthenticationService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                throw new ArgumentException("Email cannot be empty");

            if (string.IsNullOrEmpty(request.Password))
                throw new ArgumentException("Password cannot be empty");

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials");

            var claims = await _userManager.GetClaimsAsync(user);

            await _signInManager.SignInWithClaimsAsync(user, isPersistent: request.RememberMe, claims);
        }

        public async Task LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            await _signInManager.SignOutAsync();
        }

      
    }
}
