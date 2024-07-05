using HackerNewsApiWeb.Models;

namespace HackerNewsApiWeb.Repository
{
    public interface IHackerNewsRepository 
    {
        Task<List<NewsStory>> GetNewStoriesAsync();
    }
}
