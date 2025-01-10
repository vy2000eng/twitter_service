using System.Text;
using System.Text.Json;

namespace twitter_service.Service;

public class OpenAiService
{
    private readonly HttpClient _client;
    private const string API_URL = "https://api.openai.com/v1/chat/completions";
    
    public OpenAiService(IConfiguration configuration)
    {
        // _client = new HttpClient();
        // _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["openai_api_key"]}");
        
        _client = new HttpClient();
       // _client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["openai_api_key"]}");
        
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://api.openai.com/");
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer  {configuration["openai_api_key"]}");
    }
    
    


    public async Task<string> TLDRArticle(string url)
    {
        var requestBody = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = new[]
                    {
                        new
                        {
                            type = "text",
                            text =
                                "Give me the TLDR. The Main Point. 2 Sentences Max!\""
                        }
                    }
                },
                new
                {
                    role = "user",
                    content = new[]
                    {
                        new
                        {
                            type = "text",
                            text = url
                        }
                    }
                }
            },
            temperature = 1,
            max_tokens = 2048,
            top_p = 1,
            frequency_penalty = 0,
            presence_penalty = 0,
            response_format = new
            {
                type = "text"
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            // var response = await _client.PostAsync("v1/chat/completions", content);
            // response.EnsureSuccessStatusCode();
            // return await response.Content.ReadAsStringAsync();
            var response = await _client.PostAsync("v1/chat/completions", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(responseString);
            return document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "No response content found";
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"API request failed: {ex.Message}", ex);
        }
    }
}