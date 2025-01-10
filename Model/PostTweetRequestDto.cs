using Newtonsoft.Json;

namespace twitter_service.Model;

public class PostTweetRequestDto
{
    [JsonProperty("text")]
    public string Text { get; set; } = String.Empty;
}