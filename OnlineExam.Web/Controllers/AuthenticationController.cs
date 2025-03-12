namespace OnlineExam.Web.Controllers
{
    [EnableRateLimiting(RateLimiterType.Concurrency)]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly IValidator<LoginRequest> _loginValidator;

        public AuthenticationController(
            IAuthenticationService authService,
            IValidator<LoginRequest> loginValidator)
        {
            _authService = authService;
            _loginValidator = loginValidator;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginRequest());
        }

        [HttpPost("Authentication/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return View(request);
            }

            try
            {
                await _authService.LoginAsync(request);
                
                return RedirectToAction("Index", "Home"); // Redirect after login
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(request);
            }
        }

        [HttpPost("Authentication/Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.Claims.FirstOrDefault(x=> x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            await _authService.LogoutAsync(userId);
            return RedirectToAction(nameof(Login));
        }
    }
}
