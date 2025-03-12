namespace OnlineExam.Core.ViewModels.Exam.Response
{
    public class ExamWithQuestionViewModel
    {
        public ExamViewModel Exam { get; set; } = null!;
        public int AttemptId { get; set; }
        public List<UserAnswerRequest> UserAnswers { get; set; } = new();
    }
}
