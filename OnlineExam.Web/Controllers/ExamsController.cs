namespace OnlineExam.Web.Controllers
{
    [EnableRateLimiting(RateLimiterType.Concurrency)]
    public class ExamsController : Controller
    {
        private readonly IExamService _examService;
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;
        public ExamsController(IExamService examService, ISubjectService subjectService, IMapper mapper)
        {
            _examService = examService;
            _subjectService = subjectService;
            _mapper = mapper;
        }

        [HttpGet("Exams/Index")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<PaginatedResponse<ExamViewModel>>> Index(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = await _examService.GetExams(pageNumber, pageSize);

            return View(query);
        }

        [HttpGet("Exams/AvailableExams")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<ActionResult<PaginatedResponse<ExamViewModel>>> AvailableExams(
          [FromQuery] int pageNumber = 1,
          [FromQuery] int pageSize = 10)
        {
            var query = await _examService.GetExams(pageNumber, pageSize);

            return View(query);
        }

        [HttpGet("{examId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ExamViewModel>> Details(int examId)
        {
            try
            {
                var exam = await _examService.GetExamByIdAsync(examId);
                return View(exam);
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Exams/Create")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Create()
        {
            var query = await _subjectService.GetSubjectsAsync();
            ViewBag.Subjects = query.Data;
            return View(new CreateExamViewModel());
        }

        [HttpPost("Exams/Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateExam(CreateExamViewModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                await _examService.CreateExam(model, cancellationToken);
                TempData["SuccessMessage"] = "Exam created successfully!";
                return RedirectToAction("Index");
            }
            catch (BadRequest ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var query = await _subjectService.GetSubjectsAsync();
                ViewBag.Subjects = query.Data;
                return View(model);
            }
        }

        [HttpGet("Exams/Edit/{examId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> EditExamForm(int examId)
        {
            try
            {
                var exam = await _examService.GetExamByIdAsync(examId);

                var model = _mapper.Map<CreateExamViewModel>(exam);
                var query = await _subjectService.GetSubjectsAsync();
                ViewBag.Subjects = query.Data;
                ViewBag.ExamId = exam.Id;

                return View(model);
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Exams/Edit/{examId}")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int examId, CreateExamViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Show form with validation errors
            }

            try
            {
                await _examService.EditExam(examId, model, cancellationToken);
                TempData["SuccessMessage"] = "Exam updated successfully!";
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (BadRequest ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var query = await _subjectService.GetSubjectsAsync();
                ViewBag.Subjects = query.Data;

                return View(model);
            }
        }

        [HttpGet("Exams/Delete/{examId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int examId)
        {
            try
            {
                var exam = await _examService.GetExamByIdAsync(examId);
                return View(exam);
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteConfirmed(int examId, CancellationToken cancellationToken)
        {
            try
            {
                await _examService.DeleteExam(examId, cancellationToken);
                TempData["SuccessMessage"] = "Exam deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet("Exams/{examId}/Questions/Choose/Create")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult CreateChooseQuestion(int examId)
        {
            return View(new CreateChooseQuestionViewModel { ExamId = examId });
        }

        [HttpPost("Exams/{examId}/Questions/Choose/Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateChooseQuestion(int examId, CreateChooseQuestionViewModel model,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            // ✅ Check if Choices are empty
            if (model.Choices == null || model.Choices.Count == 0)
            {
                TempData["ErrorMessage"] = "You must add at least one choice.";
                return View(model);
            }

            try
            {
                await _examService.AddChooseQuestionToExam(examId, model, cancellationToken);
                TempData["SuccessMessage"] = "Choose question added successfully!";
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (ItemAlreadyExist ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (BadRequest ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet("Exams/{examId}/Questions/Choose/Edit/{questionId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> EditChooseQuestion(int examId, int questionId)
        {
            try
            {
                var exam = await _examService.GetExamByIdAsync(examId);
                var question = exam.ChooseQuestions?.FirstOrDefault(q => q.Id == questionId)
                    ?? throw new ItemNotFound("Question not found");
                var model = _mapper.Map<CreateChooseQuestionViewModel>(question);
                model.ExamId = examId;
                ViewBag.QuestionId = questionId;
                return View(model);
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { examId });
            }
        }

        [HttpPost("Exams/{examId}/Questions/Choose/Edit/{questionId}")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> EditChooseQuestion(int examId, int questionId,
            CreateChooseQuestionViewModel model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _examService.UpdateChooseQuestionToExam(examId, questionId, model, cancellationToken);
                TempData["SuccessMessage"] = "Choose question updated successfully!";
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (BadRequest ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.QuestionId = questionId;
                return View(model);
            }
        }

        [HttpPost("Exams/{examId}/Questions/Choose/Delete/{questionId}")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteChooseQuestion(int examId, int questionId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _examService.DeleteChooseQuestionToExam(examId, questionId, cancellationToken);
                TempData["SuccessMessage"] = "Choose question deleted successfully!";
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (BadRequest ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { examId });
            }
        }

        // True/False Question Actions
        [HttpGet("Exams/{examId}/Questions/TrueFalse/Create")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult CreateTrueFalseQuestion(int examId)
        {
            return View(new CreateTrueOrFalseQuestion { ExamId = examId });
        }

        [HttpPost("Exams/{examId}/Questions/TrueFalse/Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateTrueFalseQuestion(int examId, CreateTrueOrFalseQuestion model,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _examService.AddTrueOrFalseQuestionToExam(examId, model, cancellationToken);
                TempData["SuccessMessage"] = "True/False question added successfully!";
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (ItemAlreadyExist ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (BadRequest ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet("Exams/{examId}/Questions/TrueFalse/Edit/{questionId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> EditTrueFalseQuestion(int examId, int questionId)
        {
            try
            {
                var exam = await _examService.GetExamByIdAsync(examId);
                var question = exam.TrueOrFalseQuestions?.FirstOrDefault(q => q.Id == questionId)
                    ?? throw new ItemNotFound("Question not found");
                var model = _mapper.Map<CreateTrueOrFalseQuestion>(question);
                model.ExamId = examId;
                ViewBag.QuestionId = questionId;

                return View(model);
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { examId });
            }
        }

        [HttpPost("Exams/{examId}/Questions/TrueFalse/Edit/{questionId}")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> EditTrueFalseQuestion(int examId, int questionId,
            CreateTrueOrFalseQuestion model, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _examService.UpdateTrueOrFalseQuestionToExam(examId, questionId, model, cancellationToken);
                TempData["SuccessMessage"] = "True/False question updated successfully!";
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (BadRequest ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost("Exams/{examId}/Questions/TrueFalse/Delete/{questionId}")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteTrueFalseQuestion(int examId, int questionId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _examService.DeleteTrueOrFalseQuestionToExam(examId, questionId, cancellationToken);
                TempData["SuccessMessage"] = "True/False question deleted successfully!";
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (ItemNotFound ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { examId });
            }
            catch (BadRequest ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { examId });
            }
        }

    }
}
