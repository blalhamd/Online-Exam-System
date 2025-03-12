namespace OnlineExam.Domain.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();
    }
}
