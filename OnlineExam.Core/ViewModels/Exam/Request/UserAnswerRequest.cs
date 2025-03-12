namespace OnlineExam.Core.ViewModels.Exam.Request
{
    public class UserAnswerRequest
    {
        public int QuestionId { get; set; }
        public int SelectedChoiceId { get; set; } // Stores answer for MCQ or True/False (1=True, 2=False)
    }
}
