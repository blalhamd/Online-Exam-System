namespace OnlineExam.Core.IServices
{
    public interface IExamService
    {
        Task CreateExam(CreateExamViewModel model, CancellationToken cancellationToken = default);
        Task EditExam(int examId, CreateExamViewModel model, CancellationToken cancellationToken = default);
        Task DeleteExam(int examId, CancellationToken cancellationToken = default);
        Task<PaginatedResponse<ExamViewModel>> GetExams(int pageNumber = 1, int pageSize = 1);
        Task<ExamViewModel> GetExamByIdAsync(int examId);
        Task AddChooseQuestionToExam(int examId, CreateChooseQuestionViewModel model, CancellationToken cancellation = default);
        Task AddTrueOrFalseQuestionToExam(int examId, CreateTrueOrFalseQuestion model, CancellationToken cancellation = default);
        Task UpdateChooseQuestionToExam(int examId, int questionId, CreateChooseQuestionViewModel model, CancellationToken cancellation = default);
        Task UpdateTrueOrFalseQuestionToExam(int examId, int questionId, CreateTrueOrFalseQuestion model, CancellationToken cancellation = default);
        Task DeleteChooseQuestionToExam(int examId, int questionId, CancellationToken cancellation = default);
        Task DeleteTrueOrFalseQuestionToExam(int examId, int questionId, CancellationToken cancellation = default);
    }
}
