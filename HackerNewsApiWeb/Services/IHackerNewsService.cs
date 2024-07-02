using HackerNewsApiWeb.Models;

namespace HackerNewsApiWeb.Services
{
    public interface IHackerNewsService
    {
        Task<List<NewsStory>> GetNewStoriesAsync();        
    }
}
