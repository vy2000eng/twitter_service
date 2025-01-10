using Microsoft.EntityFrameworkCore;
using textyService.Data;
using twitter_service;
using twitter_service.Data;
using twitter_service.Service;



var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<textyContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("prod_connection_string");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'prod_connection_string' not found.");
    }
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
builder.Services.AddIdentityCore<textyUser>()
    .AddEntityFrameworkStores<textyContext>();

builder.Services.AddHostedService<Worker>();

// Services that should be Scoped or Transient, not Singleton
builder.Services.AddScoped<ITwitterService, TwitterService>();
builder.Services.AddScoped<OpenAiService>();



var host = builder.Build();
host.Run();;;