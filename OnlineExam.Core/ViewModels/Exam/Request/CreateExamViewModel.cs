namespace OnlineExam.Core.ViewModels.Exam.Request
{
    public class CreateExamViewModel
    {
        public int SubjectId { get; set; }
        public int TotalGrade { get; set; }
        public int Level { get; set; }
        public TimeOnly Duration { get; set; }
        public ExamType ExamType { get; set; }
        public string Description { get; set; } = null!;
        public bool Status { get; set; } // (Active) Or (Not Active)
        public List<CreateTrueOrFalseQuestion> TrueOrFalseQuestions { get; set; } = new List<CreateTrueOrFalseQuestion>();
        public List<CreateChooseQuestionViewModel> ChooseQuestions { get; set; } = new List<CreateChooseQuestionViewModel>();
    }
}
