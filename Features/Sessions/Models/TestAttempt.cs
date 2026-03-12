namespace TestingPlatform.Features.Sessions.Models;

using TestingPlatform.Features.Users.Models;

public class TestAttempt
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }

    public int TestSessionId { get; set; }
    public TestSession? TestSession { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public int TotalScore { get; set; }
    public int MaxPossibleScore { get; set; }

    public ICollection<AttemptAnswer> Answers { get; set; } = new List<AttemptAnswer>();
}