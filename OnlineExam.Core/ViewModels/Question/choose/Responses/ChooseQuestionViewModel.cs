namespace OnlineExam.Core.ViewModels.Question.choose.Responses
{
    public class ChooseQuestionViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int ExamId { get; set; }
        public double GradeOfQuestion { get; set; }
        public List<ChoiceViewModel> Choices { get; set; } = new List<ChoiceViewModel>();
        public int CorrectAnswerIndex { get; set; }
    }
}
