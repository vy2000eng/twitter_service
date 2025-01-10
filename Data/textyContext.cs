using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using textyService.Data;
using twitter_service.Model;

namespace twitter_service.Data;

public class textyContext : IdentityDbContext<textyUser>
{
    public textyContext(DbContextOptions<textyContext> options)
        : base(options)
    {
    }

    //public DbSet<NewsFeedModelContainer> NewsFeedModelContainers { get; set; }
    public DbSet<NewsFeedModel> NewsFeedModels { get; set; }
    public DbSet<UserSavedArticle> UserSavedArticles { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<NewsFeedModel>()
            .HasMany(usa => usa.SavedByUsers)
            .WithOne(nfm => nfm.NewsFeedModel)
            .HasForeignKey(nfm => nfm.NewsFeedModelId);

        //.WithOne(newsModel => newsModel.) 
        builder.Entity<UserSavedArticle>()
            .HasOne(usa => usa.User)
            .WithMany(user => user.SavedArticles)
            .HasForeignKey(usa => usa.UserId);

        builder.Entity<textyUser>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        builder.Entity<textyUser>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}