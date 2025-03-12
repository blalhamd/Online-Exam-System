namespace OnlineExam.Web.Controllers
{
    [Authorize(Policy = "AdminOrUser")]
    [EnableRateLimiting(RateLimiterType.Concurrency)]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService 
            userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // ✅ 1️⃣ View Exam & Questions
        [HttpGet("Exams/{examId}")]
        public async Task<IActionResult> GetExamWithQuestions(int examId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                    return Unauthorized("User not found.");

                var response = await _userService.GetExamWithQuestions(examId, userId, cancellationToken);
                return View("ExamView", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve exam {ExamId}", examId);
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Exams/Submit/{examAttemptId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitExam(int examAttemptId, ExamSubmissionRequest request)
        {
            if (request.Answers == null || !request.Answers.Any())
            {
                _logger.LogError("No answers were received in the request.");
                TempData["ErrorMessage"] = "Please answer all questions before submitting.";
                return RedirectToAction(nameof(GetExamWithQuestions), new { examId = examAttemptId });
            }

            try
            {
                var result = await _userService.Submit(examAttemptId, request);
                return View("ExamResult", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to submit exam attempt {ExamAttemptId}", examAttemptId);
                return BadRequest(ex.Message);
            }
        }

    }
}
