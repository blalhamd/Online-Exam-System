namespace OnlineExam.Domain.Entities
{
    public class UserAnswer : BaseEntity
    {
        public int ExamAttemptId { get; set; }
        public ExamAttempt ExamAttempt { get; set; } = null!;

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public int? SelectedChoiceId { get; set; } // For MCQ questions
        public Choice? SelectedChoice { get; set; }

        public bool? TrueFalseAnswer { get; set; } // True/False for TrueOrFalseQuestions

        public double Score { get; set; } = 0; // Calculated based on correctness
    }
}
