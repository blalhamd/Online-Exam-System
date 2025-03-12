namespace OnlineExam.Core.ViewModels.Question.choose.Requests
{
    public class CreateChooseQuestionViewModel
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = null!;
        public double GradeOfQuestion { get; set; }
       
        [BindProperty]
        public List<CreateChoiceViewModel> Choices { get; set; } = new List<CreateChoiceViewModel>();
        public int CorrectAnswerIndex { get; set; }
    }
}
