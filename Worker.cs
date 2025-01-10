using twitter_service.Service;

namespace twitter_service;

public class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<Worker> _logger;
    

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var leftTimes = new[] { 800, 930, 1100, 1230, 1400, 1530, 1700, 1830 };    // Left news times
        var rightTimes = new[] { 845, 1015, 1145, 1315, 1445, 1615, 1745, 1915 };

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var currentTime = now.Hour * 100 + now.Minute;

            // Find next post time
            var nextLeftTime = leftTimes.FirstOrDefault(t => t > currentTime);
            var nextRightTime = rightTimes.FirstOrDefault(t => t > currentTime);

            // If no times left today, use first time tomorrow
            if (nextLeftTime == 0 && nextRightTime == 0)
            {
                nextLeftTime = leftTimes[0];
                nextRightTime = rightTimes[0];
                now = now.Date.AddDays(1);
            }

            // Calculate next run time based on whichever comes first
            var nextRunTime = (nextLeftTime == 0 || (nextRightTime != 0 && nextRightTime < nextLeftTime)) 
                ? nextRightTime 
                : nextLeftTime;

            var nextRun = now.Date.AddHours(nextRunTime / 100).AddMinutes(nextRunTime % 100);
            var delay = nextRun - DateTime.Now;

            _logger.LogInformation($"Next post scheduled for: {nextRun}");
            await Task.Delay(delay, stoppingToken);

            using (var scope = _serviceProvider.CreateScope())
            {
                var twitterService = scope.ServiceProvider.GetRequiredService<ITwitterService>();
            
                // Determine if it's a left post time
                var isLeftPost = leftTimes.Contains(nextRunTime);
            
                if (isLeftPost)
                {
                    _logger.LogInformation("Making left post");
                    await twitterService.MakeLeftPost();
                }
                else
                {
                    _logger.LogInformation("Making right post");
                    await twitterService.MakeRightPost();
                }
            }
        }
    }
}