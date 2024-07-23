using HackerNewsApi.Data.Models;

namespace Repository.Service
{
    public interface IHackerNewsService
    {
        Task<List<NewsStory>> GetNewStoriesAsync();        
    }
}
