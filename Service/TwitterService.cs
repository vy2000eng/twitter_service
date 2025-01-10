using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Tweetinvi;
using Tweetinvi.Models;
using twitter_service.Data;
using twitter_service.Model;

namespace twitter_service.Service;

public class TwitterService : ITwitterService
{
    private readonly string _ApiKey;
    private readonly string _ApiSecret;
    private readonly string _accessToken;
    private readonly string _accessTokenSecret;
    private readonly string _AnalysisApiEndpoint;
    //private readonly string _TwitterApiEndpoint;
    private readonly textyContext _context;
    private readonly OpenAiService _openAiService;
    

    public TwitterService(IConfiguration configuration, textyContext context, OpenAiService openAiService)
    {
        _context             = context;
        _openAiService       = openAiService;
        _accessToken         = configuration["Twitter:AccessToken"];
        _accessTokenSecret   = configuration["Twitter:AccessTokenSecret"];
        _ApiKey              = configuration["Twitter:ApiKey"];
        _ApiSecret           = configuration["Twitter:ApiKeySecret"];
        _AnalysisApiEndpoint = configuration["Twitter:BaseAnalysisApiUrl"];
    }

    public async Task ExposingApiCall()
    {
        var leftNewsTweet = new PostTweetRequestDto
        {
            Text = await GetTldrAnalysis(1)
         
        };
        
        var rightNewsTweet= new PostTweetRequestDto
        {
            Text = await GetTldrAnalysis(2)
         
        };
        await PostTweetAsync(leftNewsTweet);
        await PostTweetAsync(rightNewsTweet);
    }

    public async Task<bool> PostTweetAsync(PostTweetRequestDto msg)
    {
        try
        {
            var userClient = new TwitterClient(_ApiKey,_ApiSecret ,_accessToken,_accessTokenSecret);
            var result = await userClient.Execute.AdvanceRequestAsync(BuildTwitterRequest(msg,userClient));
          

            //var tweet = await userClient.Tweets.PublishTweetAsync(msg);
            return result != null;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine(ex.Message);
            return false;
        }
    }

   private static Action<ITwitterRequest> BuildTwitterRequest(PostTweetRequestDto newTweet, TwitterClient client)
    {
        return (ITwitterRequest request) =>
        {
            var jsonBody = client.Json.Serialize(newTweet);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            request.Query.Url = "https://api.twitter.com/2/tweets";
            request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
            request.Query.HttpContent = content;
        };
    }



    private async Task<string> GetTldrAnalysis(int leftRight)
    {
        var  article = await _context.NewsFeedModels
            .Where(nf => nf.LeftRightId == leftRight)
            .OrderByDescending(nf => nf.PublishDate)
            .FirstOrDefaultAsync();
        HttpClient client = new HttpClient();
       var response = await client.GetAsync(
           $"{_AnalysisApiEndpoint}{article.Id}?articleUrl={Uri.EscapeDataString(article.Link)}");
       var result = new TwitterPost
       {
           LeftRightNews = leftRight==1 ? "Left News": "Right News",
           AnalysisUrl = response.RequestMessage.RequestUri.ToString(),
           TldrSummary = await _openAiService.TLDRArticle(article.Link),
           ArticleUrl = article.Link
       };

       return "\t" + result.LeftRightNews + " - " + result.AnalysisUrl +
              "\n" + result.TldrSummary +
              "\n" + result.ArticleUrl;





    }


}