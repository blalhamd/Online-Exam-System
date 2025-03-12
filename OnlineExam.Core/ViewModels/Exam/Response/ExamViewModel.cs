namespace OnlineExam.Core.ViewModels.Exam.Response
{
    public class ExamViewModel // will inclue Subject and select Name it..
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public int TotalGrade { get; set; }
        public int Level { get; set; }
        public TimeOnly Duration { get; set; }
        public ExamType ExamType { get; set; }
        public string Description { get; set; } = null!;
        public bool Status { get; set; } // (Active) Or (Not Active)
        public ICollection<ChooseQuestionViewModel>? ChooseQuestions { get; set; } = new List<ChooseQuestionViewModel>();
        public ICollection<TrueOrFalseQuestion>? TrueOrFalseQuestions { get; set; } = new List<TrueOrFalseQuestion>();
        public int? NumberOfQuestions { get; set; }
    }
}
