using IBM.Cloud.SDK.Core.Authentication.Iam;
using IBM.Watson.NaturalLanguageUnderstanding.v1;
using IBM.Watson.NaturalLanguageUnderstanding.v1.Model;
using twitter_service.Model;



namespace twitter_service.Service;

public class IbmWatsonNluService
{
    private readonly NaturalLanguageUnderstandingService _naturalLanguageUnderstanding;

    public IbmWatsonNluService(IConfiguration configuration)
    {
        IamAuthenticator authenticator = new IamAuthenticator(apikey: configuration["api_key"]);
        _naturalLanguageUnderstanding = new NaturalLanguageUnderstandingService("2022-04-07", authenticator);
        _naturalLanguageUnderstanding.SetServiceUrl(configuration["url"]);

        // private readonly


    }


    public string AnalyzeText(string text, int entityLimit)
    {

        var result = _naturalLanguageUnderstanding.Analyze(
            text: text,
            features: new Features()
            {
                Keywords = new KeywordsOptions()
                {
                    //Sentiment = true,
                    Emotion = true,
                    Limit = entityLimit

                },
                Sentiment = new SentimentOptions()
                {
                    //Targets = new List<string>() { "France" }
                },
                Entities = new EntitiesOptions()
                {
                    Sentiment = true,
                    Limit = entityLimit
                },

              
            }
        );
        return result.Response;
    }
    
    public string AnalyzeUrl(string urlStr)
    {

        var result = _naturalLanguageUnderstanding.Analyze(
            url: urlStr,
            features: new Features()
            {
                Keywords = new KeywordsOptions()
                {
                    //Sentiment = true,
                    Emotion = true,
                    Limit = 5

                },
            }
        );
        return result.Response;

    }

    public string AnalyzeSentimentFromURL(string urlStr)
    {
        try
        {
            var results = _naturalLanguageUnderstanding.Analyze(
                url: urlStr,
                features: new Features()
                {
                    Sentiment = new SentimentOptions()
                    {
                        //Targets = new List<string>() { "France" }
                    }
                }
            );

            // Check if the response is null or empty
            if (string.IsNullOrEmpty(results.Response))
            {
                return "Error: No response from the sentiment analysis service.";
            }

            return results.Response;
        }
        catch (Exception e)
        {
            // Handle any other exceptions
            return $"Error analyzing sentiment: {e.Message}";
        }
    }
    

}