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
        
        var executionTimes = new[] { 8, 10, 12, 14, 16, 18, 20,22 };  // 7 posts daily

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRunHour = executionTimes
                .Where(h => h > now.Hour)
                .Cast<int?>()
                .FirstOrDefault() ?? executionTimes[0];
            
            var nextRun = now.Date.AddHours(nextRunHour);
            
            if (nextRunHour <= now.Hour)
                nextRun = nextRun.AddDays(1);
            
            var delay = nextRun - now;
            await Task.Delay(delay, stoppingToken);

            using (var scope = _serviceProvider.CreateScope())
            {
                var twitterService = scope.ServiceProvider.GetRequiredService<ITwitterService>();
                await twitterService.ExposingApiCall();
            }
            //Environment.Exit(0);

        }
        
    }
}