namespace OnlineExam.Core.AutoMapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            // ✅ Mapping for CreateExam action
            CreateMap<CreateExamViewModel, Exam>();

            CreateMap<CreateChooseQuestionViewModel, ChooseQuestion>();
            CreateMap<CreateChoiceViewModel, Choice>().ReverseMap(); // ✅ Bidirectional mapping

            // ✅ Mapping for EditExam action
            CreateMap<CreateExamViewModel, Exam>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Keep existing ID
                .ForMember(dest => dest.ChooseQuestions, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.TrueOrFalseQuestions, opt => opt.Ignore());
            // ✅ Mapping for retrieving an exam (Exam → ExamViewModel)
            CreateMap<Exam, ExamViewModel>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(x => x.Subject.Name))
                .ForMember(dest => dest.ChooseQuestions, opt => opt.MapFrom(x => x.ChooseQuestions));

            CreateMap<ChooseQuestion, ChooseQuestionViewModel>().ReverseMap();
            CreateMap<TrueOrFalseQuestion, CreateTrueOrFalseQuestion>().ReverseMap();

            CreateMap<ExamViewModel, CreateExamViewModel>().ReverseMap();

            CreateMap<Choice, ChoiceViewModel>().ReverseMap();
            CreateMap<CreateChoiceViewModel,Choice>();

            CreateMap<ChooseQuestionViewModel, CreateChooseQuestionViewModel>().ReverseMap();
            CreateMap<ChoiceViewModel, CreateChoiceViewModel>().ReverseMap();
        }
    }
}
