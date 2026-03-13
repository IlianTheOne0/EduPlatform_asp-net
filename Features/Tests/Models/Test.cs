namespace TestingPlatform.Features.Tests.Models;

using TestingPlatform.Features.Users.Models;
using TestingPlatform.Features.Sessions.Models;

public class Test
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TestStatus Status { get; set; } = TestStatus.Draft;


    public int TimeLimitMinutes { get; set; } = 0;
    public bool ShowResultsAtEnd { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public string CreatorId { get; set; } = string.Empty;
    public User? Creator { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<TestSession> Sessions { get; set; } = new List<TestSession>();

    public override string ToString() => $"Test: {Title} | Status: {Status} | Time Limit: {TimeLimitMinutes}m";
}