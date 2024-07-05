using HackerNewsApiWeb.Models;
using HackerNewsApiWeb.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace HackerNewsApiWeb.Services
{
    public class HackerNewsService : IHackerNewsService
    {
      
        private readonly IHackerNewsRepository _hackerNewsRepository;

        public HackerNewsService(IHackerNewsRepository hackerNewsRepository)
        {
            _hackerNewsRepository = hackerNewsRepository;
        }
        
        public async Task<List<NewsStory>> GetNewStoriesAsync()
        {
            return await _hackerNewsRepository.GetNewStoriesAsync();
        }
      

 
    }
}
