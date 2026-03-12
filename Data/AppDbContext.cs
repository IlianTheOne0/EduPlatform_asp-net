namespace TestingPlatform.Data;

using TestingPlatform.Features.Sessions.Models;
using TestingPlatform.Features.Tests.Models;
using TestingPlatform.Features.Users.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<User>
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<AnswerOption> AnswerOptions { get; set; }
    public DbSet<TestSession> TestSessions { get; set; }
    public DbSet<TestAttempt> TestAttempts { get; set; }
    public DbSet<AttemptAnswer> AttemptAnswers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasIndex(category => category.Name)
            .IsUnique();

        modelBuilder.Entity<Test>()
            .HasOne(test => test.Category)
            .WithMany(category => category.Tests)
            .HasForeignKey(test => test.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Test>()
            .HasOne(test => test.Creator)
            .WithMany()
            .HasForeignKey(test => test.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TestSession>()
            .HasIndex(session => session.JoinCode)
            .IsUnique();

        modelBuilder.Entity<TestSession>()
            .HasOne(session => session.Test)
            .WithMany(test => test.Sessions)
            .HasForeignKey(session => session.TestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestAttempt>()
            .HasOne(attempt => attempt.TestSession)
            .WithMany(session => session.Attempts)
            .HasForeignKey(attempt => attempt.TestSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestAttempt>()
            .HasOne(attempt => attempt.User)
            .WithMany()
            .HasForeignKey(attempt => attempt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttemptAnswer>()
            .HasOne(answer => answer.TestAttempt)
            .WithMany(attempt => attempt.Answers)
            .HasForeignKey(answer => answer.TestAttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AttemptAnswer>()
            .HasOne(answer => answer.Question)
            .WithMany()
            .HasForeignKey(answer => answer.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}