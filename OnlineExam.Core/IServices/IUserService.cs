namespace OnlineExam.Core.IServices
{
    public interface IUserService
    {
        Task<ExamWithQuestionViewModel> GetExamWithQuestions(int examId, string userId, CancellationToken cancellation = default);
        Task<ScoreViewModel> Submit(int examAttemptId, ExamSubmissionRequest request); // POST /exams/submit/{examAttemptId}
    }
}
