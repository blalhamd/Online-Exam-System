namespace OnlineExam.Core.ViewModels.Exam.Request
{
    public class ExamSubmissionRequest
    {
        public List<UserAnswerRequest> Answers { get; set; } = new();
    }
}
