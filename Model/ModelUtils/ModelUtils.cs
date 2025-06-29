using System.Text.Json;

namespace twitter_service.Model.ModelUtils;

public static class ModelUtilsClass
{
    public static Sentiment ParseSentiment(string sentiment)
    {
        using JsonDocument document = JsonDocument.Parse(sentiment);
        var root = document.RootElement;
        var sentimentKey = root.GetProperty("sentiment")
            .GetProperty("document");
        var sentimentModel = new Sentiment()
        {
            Score = sentimentKey.GetProperty("score").GetDouble(),
            Label = sentimentKey.GetProperty("label").GetString()


        };
        return sentimentModel;
    }
    
    public static List<Keywords> ExtractKeywords(string jsonResponse)
    {
        using JsonDocument doc = JsonDocument.Parse(jsonResponse);
        var keywordsArray = doc.RootElement.GetProperty("keywords");
    
        var keywords = new List<Keywords>();
        foreach (var element in keywordsArray.EnumerateArray())
        {
            keywords.Add(new Keywords
            {
                Keyword = element.GetProperty("text").GetString(),
                Relevance = element.GetProperty("relevance").GetDouble()
            });
        }
    
        return keywords;
    }
    public static string ExtractPublisherFromUrl(string url)
    {
        try
        {
            Uri uri = new Uri(url);
            string host = uri.Host;
        
            // Remove 'www.' if present
            if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                host = host.Substring(4);
            }
        
            // Extract the domain name (e.g., 'cnn.com' from 'www.cnn.com')
            string[] parts = host.Split('.');
            if (parts.Length >= 2)
            {
                return parts[parts.Length - 2].ToUpper();
            }
        
            return host.ToUpper();
        }
        catch
        {
            return "UNKNOWN";
        }
    }
}