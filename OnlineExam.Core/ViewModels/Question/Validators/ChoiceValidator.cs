using OnlineExam.Core.ViewModels.Question.choose.Requests;

namespace OnlineExam.Core.ViewModels.Question.Validators
{
    public class ChoiceValidator : AbstractValidator<CreateChoiceViewModel>
    {
        public ChoiceValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Choice text cannot be empty.");
        }
    }
}
