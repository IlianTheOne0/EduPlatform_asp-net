namespace TestingPlatform.Features.Sessions.Services;

using TestingPlatform.Data;
using TestingPlatform.Features.Sessions.Interfaces;

using Microsoft.EntityFrameworkCore;

public class AttemptTimeoutWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AttemptTimeoutWorker> _logger;

    public AttemptTimeoutWorker(IServiceProvider serviceProvider, ILogger<AttemptTimeoutWorker> logger) { _serviceProvider = serviceProvider; _logger = logger; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await ProcessAbandonedAttemptsAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Error occurred processing abandoned attempts."); }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ProcessAbandonedAttemptsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var attemptService = scope.ServiceProvider.GetRequiredService<ITestAttemptService>();

        var now = DateTime.UtcNow;

        var abandonedAttempts = await context.TestAttempts
            .Include(attempt => attempt.TestSession!)
            .ThenInclude(session => session.Test)
            .Where(attempt => attempt.CompletedAt == null && attempt.TestSession!.Test!.TimeLimitMinutes > 0)
            .ToListAsync();

        foreach (var attempt in abandonedAttempts)
        {
            var expirationTime = attempt.StartedAt.AddMinutes(attempt.TestSession!.Test!.TimeLimitMinutes);

            if (expirationTime.AddMinutes(1) < now)
            {
                _logger.LogInformation("Auto-completing abandoned attempt {AttemptId} for Session {SessionId}", attempt.Id, attempt.TestSessionId);
                await attemptService.AutoCompleteAbandonedAttemptAsync(attempt.Id);
            }
        }
    }
}