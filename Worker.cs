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
    // Optimized posting times spread throughout the day (8 pairs)
    // Times are in 24-hour format (HHMM)
    var leftTimes = new[] { 900, 1100, 1300, 1500, 1700, 1900, 2100, 2300 };    // Left news times
    var rightTimes = new[] { 915, 1115, 1315, 1515, 1715, 1915, 2115, 2315 };   // 15 minutes after each left post

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
        
        try 
        {
            await Task.Delay(delay, stoppingToken);

            using (var scope = _serviceProvider.CreateScope())
            {
                var twitterService = scope.ServiceProvider.GetRequiredService<ITwitterService>();

                // Determine if it's a left post time based on the current time
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
        catch (OperationCanceledException)
        {
            // Graceful shutdown
            _logger.LogInformation("Twitter posting service is shutting down");
            break;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while posting to Twitter");
            // Wait a bit before retrying
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}


}

//
// protected override async Task ExecuteAsync(CancellationToken stoppingToken)
// {
//     // Optimized posting times spread throughout the day (8 pairs)
//     // Times are in 24-hour format (HHMM)
//     var leftTimes = new[] { 900, 1100, 1300, 1500, 1700, 1900, 2100, 2300 };    // Left news times
//     var rightTimes = new[] { 915, 1115, 1315, 1515, 1715, 1915, 2115, 2315 };   // 15 minutes after each left post
//
//     while (!stoppingToken.IsCancellationRequested)
//     {
//         var now = DateTime.Now;
//         var currentTime = now.Hour * 100 + now.Minute;
//
//         // Find next post time
//         var nextLeftTime = leftTimes.FirstOrDefault(t => t > currentTime);
//         var nextRightTime = rightTimes.FirstOrDefault(t => t > currentTime);
//
//         // If no times left today, use first time tomorrow
//         if (nextLeftTime == 0 && nextRightTime == 0)
//         {
//             nextLeftTime = leftTimes[0];
//             nextRightTime = rightTimes[0];
//             now = now.Date.AddDays(1);
//         }
//
//         // Calculate next run time based on whichever comes first
//         var nextRunTime = (nextLeftTime == 0 || (nextRightTime != 0 && nextRightTime < nextLeftTime)) 
//             ? nextRightTime 
//             : nextLeftTime;
//
//         var nextRun = now.Date.AddHours(nextRunTime / 100).AddMinutes(nextRunTime % 100);
//         var delay = nextRun - DateTime.Now;
//
//         _logger.LogInformation($"Next post scheduled for: {nextRun}");
//         
//         try 
//         {
//             await Task.Delay(delay, stoppingToken);
//
//             using (var scope = _serviceProvider.CreateScope())
//             {
//                 var twitterService = scope.ServiceProvider.GetRequiredService<ITwitterService>();
//
//                 // Determine if it's a left post time based on the current time
//                 var isLeftPost = leftTimes.Contains(nextRunTime);
//
//                 if (isLeftPost)
//                 {
//                     _logger.LogInformation("Making left post");
//                     await twitterService.MakeLeftPost();
//                 }
//                 else
//                 {
//                     _logger.LogInformation("Making right post");
//                     await twitterService.MakeRightPost();
//                 }
//             }
//         }
//         catch (OperationCanceledException)
//         {
//             // Graceful shutdown
//             _logger.LogInformation("Twitter posting service is shutting down");
//             break;
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error occurred while posting to Twitter");
//             // Wait a bit before retrying
//             await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
//         }
//     }
// }