namespace TestingPlatform.Features.Sessions.Models;

using TestingPlatform.Features.Tests.Models;
using TestingPlatform.Features.Users.Models;

public class TestSession
{
    public int Id { get; set; }
    public string JoinCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TestId { get; set; }
    public Test? Test { get; set; }

    public string TeacherId { get; set; } = string.Empty;
    public User? Teacher { get; set; }

    public ICollection<TestAttempt> Attempts { get; set; } = new List<TestAttempt>();
}