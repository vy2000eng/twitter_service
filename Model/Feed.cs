using textyService.Data;

namespace twitter_service.Model;

public class NewsFeedModel
{
    public int Id { get; set; }
    //public int ModelContainerId { get; set; }
    public int LeftRightId { get; set; }
    public string ArticleId { get; set; }
    
    // public NewsFeedModelContainer ModelContainer { get; set; }
    public string Title { get; set; }
    public string Link { get; set; }
    public DateTime PublishDate { get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    
    public string Sentiment { get; set; }
    
    public DateTime? SavedDate{ get; set; } = DateTime.UtcNow;


    // public bool IsHistory { get; set; } = false;
    
    // Navigation property for saved articles
    public List<UserSavedArticle> SavedByUsers { get; set; }
}

public class UserSavedArticle
{
    public int Id { get; set; }  // Primary key
    public int NewsFeedModelId { get; set; }  // Foreign key to NewsFeedModel
    public bool? IsHistory { get; set; } = false;

    public string UserId { get; set; }  // Foreign key to textyUser
    public DateTime SavedDate { get; set; }

    
    // Navigation properties
    public NewsFeedModel NewsFeedModel { get; set; }
    public textyUser User { get; set; }
}