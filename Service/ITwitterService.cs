using twitter_service.Model;

namespace twitter_service.Service;

public interface ITwitterService
{ 
    Task<bool>  PostTweetAsync(PostTweetRequestDto msg);

    public Task ExposingApiCall();
}