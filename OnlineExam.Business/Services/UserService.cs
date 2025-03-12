namespace OnlineExam.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepositoryAsync<Exam> _examRepository;
        private readonly IGenericRepositoryAsync<ExamAttempt> _examAttempRepository;
        private readonly IGenericRepositoryAsync<UserAnswer> _userAnswerRepository;
        private readonly IUnitOfWork<Exam> _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(
            IGenericRepositoryAsync<Exam> examRepository,
            IGenericRepositoryAsync<ExamAttempt> examAttempRepository,
            IGenericRepositoryAsync<UserAnswer> userAnswerRepository,
            IUnitOfWork<Exam> unitOfWork,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _examRepository = examRepository;
            _examAttempRepository = examAttempRepository;
            _userAnswerRepository = userAnswerRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // ✅ 1️⃣ Get Exam with Questions
        public async Task<ExamWithQuestionViewModel> GetExamWithQuestions(int examId, string userId, CancellationToken cancellationToken = default)
        {
            var exam = await _examRepository.FirstOrDefaultAsync(e => e.Id == examId && e.Status,
                q => q.Include(x => x.TrueOrFalseQuestions)
                      .Include(x => x.ChooseQuestions)!
                      .ThenInclude(cq => cq.Choices));

            if (exam is null)
            {
                _logger.LogWarning("Exam not found or inactive.");
                throw new ItemNotFound("Exam not found or inactive.");
            }

            // ✅ Check for existing attempt
            var existingAttempt = await _examAttempRepository.FirstOrDefaultAsync(a => a.UserId == userId && a.ExamId == examId && !a.IsSubmitted);

            if (existingAttempt == null)
            {
                existingAttempt = new ExamAttempt
                {
                    UserId = userId!,
                    ExamId = examId,
                    StartTime = DateTimeOffset.UtcNow,
                    IsSubmitted = false
                };

                await _examAttempRepository.AddEntityAsync(existingAttempt);
                await _unitOfWork.CommitAsync(cancellationToken);
            }

            // ✅ Map Exam to `ExamViewModel`
            var examViewModel = _mapper.Map<ExamViewModel>(exam);
            examViewModel.NumberOfQuestions = exam.NumberOfQuestions;

            return new ExamWithQuestionViewModel
            {
                AttemptId = existingAttempt.Id,
                Exam = examViewModel
            };
        }


        public async Task<ScoreViewModel> Submit(int examAttemptId, ExamSubmissionRequest request)
        {
            var attempt = await _examAttempRepository.FirstOrDefaultAsync(
                a => a.Id == examAttemptId && !a.IsSubmitted,
                a => a.Include(x => x.Exam)
                      .ThenInclude(x => x.ChooseQuestions)!
                      .ThenInclude(cq => cq.Choices)
                      .Include(x => x.Exam.TrueOrFalseQuestions));

            if (attempt == null)
            {
                _logger.LogWarning("❌ Exam attempt not found or already submitted.");
                throw new BadRequest("Exam attempt not found or already submitted.");
            }

            double totalScore = 0;
            int numberCorrectQuestions = 0, numberWrongQuestions = 0;
            List<UserAnswer> userAnswers = new();
            List<AnsweredQuestionViewModel> answeredQuestions = new();

            foreach (var answer in request.Answers)
            {
                var question = attempt.Exam.ChooseQuestions?.FirstOrDefault(q => q.Id == answer.QuestionId)
                                as Question ??
                                attempt.Exam.TrueOrFalseQuestions?.FirstOrDefault(q => q.Id == answer.QuestionId);

                if (question == null)
                {
                    _logger.LogWarning($"⚠️ Invalid question ID {answer.QuestionId}");
                    throw new BadRequest($"Invalid question ID {answer.QuestionId}");
                }

                var userAnswer = new UserAnswer
                {
                    ExamAttemptId = examAttemptId,
                    QuestionId = question.Id
                };

                bool isCorrect = false;

                if (question is ChooseQuestion mcq)
                {
                    userAnswer.SelectedChoiceId = answer.SelectedChoiceId;
                    var correctChoice = mcq.Choices.ElementAtOrDefault(mcq.CorrectAnswerIndex);

                    if (correctChoice?.Id == answer.SelectedChoiceId)
                    {
                        isCorrect = true;
                        userAnswer.Score = question.GradeOfQuestion;
                    }
                }
                else if (question is TrueOrFalseQuestion tf)
                {
                    userAnswer.TrueFalseAnswer = answer.SelectedChoiceId == 1; // 1=True, 2=False
                    isCorrect = tf.CorrectValue == userAnswer.TrueFalseAnswer;

                    if (isCorrect)
                        userAnswer.Score = question.GradeOfQuestion;
                }

                if (isCorrect)
                    numberCorrectQuestions++;
                else
                    numberWrongQuestions++;

                totalScore += userAnswer.Score;
                userAnswers.Add(userAnswer);

                // Store the question result
                answeredQuestions.Add(new AnsweredQuestionViewModel
                {
                    QuestionId = question.Id,
                    Title = question.Title,
                    ExamId = question.ExamId,
                    GradeOfQuestion = question.GradeOfQuestion,
                    IsCorrect = isCorrect
                });
            }

            await _userAnswerRepository.AddRangeAsync(userAnswers);
            attempt.Score = totalScore;
            attempt.IsSubmitted = true;
            attempt.EndTime = DateTimeOffset.UtcNow;
            await _examAttempRepository.UpdateEntityAsync(attempt);
            await _unitOfWork.CommitAsync();

            return new ScoreViewModel
            {
                Score = totalScore,
                NumberCorrectQuestions = numberCorrectQuestions,
                NumberWrongQuestions = numberWrongQuestions,
                TotalGrade = attempt.Exam.TotalGrade,
                ScoreInPercentage = (totalScore / attempt.Exam.TotalGrade) * 100,
                AnsweredQuestions = answeredQuestions,
                NumberQuestions = numberCorrectQuestions + numberWrongQuestions
            };
        }

    }

}
