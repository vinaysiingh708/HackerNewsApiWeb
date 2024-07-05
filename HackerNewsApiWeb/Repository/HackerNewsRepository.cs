using HackerNewsApiWeb.Models;
using HackerNewsApiWeb.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace HackerNewsApiWeb.Repository
{
    public class HackerNewsRepository : IHackerNewsRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private const string BaseUrl = "https://hacker-news.firebaseio.com/v0/";
        private readonly ILogger<HackerNewsRepository> _logger;

        public HackerNewsRepository(HttpClient httpClient,IMemoryCache cache, ILogger<HackerNewsRepository> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }
        public async Task<List<NewsStory>> GetNewStoriesAsync()
        {
            return await GetStoriesAsync("topstories");
        }
        private async Task<List<NewsStory>> GetStoriesAsync(string endpoint)
        {
            var cacheKey = $"{endpoint}";

            if (_cache.TryGetValue(cacheKey, out List<NewsStory> stories))
            {
                _logger.LogInformation($"Cache hit for {cacheKey}");
                return stories;
            }

            try
            {
                _logger.LogInformation($"Cache miss for {cacheKey}. Fetching from API.");

                var storiesUrl = $"{BaseUrl}{endpoint}.json";
                var storiesIds = await _httpClient.GetFromJsonAsync<List<int>>(storiesUrl);

                stories = new List<NewsStory>();
                if (storiesIds != null)
                {                   
                    var paginatedIds = storiesIds.Take(200).ToList();                                                   
                    await Task.WhenAll(
                        paginatedIds.Select(async id =>
                        {
                            var storyUrl = $"{BaseUrl}item/{id}.json";
                            var story = await _httpClient.GetFromJsonAsync<NewsStory>(storyUrl);
                            if (story != null)
                            {
                                stories.Add(story);
                            }
                        }).ToArray()
                    );                    
                }
                else
                {
                    _logger.LogInformation($"Cache hit for {cacheKey}");
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };
                _cache.Set(cacheKey, stories, cacheEntryOptions);

                return stories.ToList();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP request error for {cacheKey}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error for {cacheKey}");
                throw;
            }
        }
    }
}
