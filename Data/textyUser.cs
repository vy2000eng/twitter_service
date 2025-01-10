using Microsoft.AspNetCore.Identity;
using twitter_service.Model;

namespace textyService.Data;

public class textyUser:IdentityUser
{
    /// public int UserName { get; set; }
    public List<UserSavedArticle> SavedArticles { get; set; }

}