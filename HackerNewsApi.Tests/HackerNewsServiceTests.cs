using HackerNewsApi.Data.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using Repository.Service;
using System.Net;
using System.Text.Json;


namespace HackerNewsApiWeb.Tests
{
    public class HackerNewsServiceTests
    {
        [Fact]
        public async Task GetNewStoriesAsync_ShouldReturnEmpty_WhenApiReturnsNoData()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    if (request.RequestUri.AbsolutePath.EndsWith("topstories.json"))
                    {
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(JsonSerializer.Serialize(new List<int>()))
                        };
                    }
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://hacker-news.firebaseio.com/")
            };

            var mockMemoryCache = new Mock<IMemoryCache>();
            var cacheEntry = new Mock<ICacheEntry>();
            mockMemoryCache
                .Setup(mc => mc.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Returns(false);
            mockMemoryCache
                .Setup(mc => mc.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntry.Object);

            var mockLogger = new NullLogger<HackerNewsService>();

            var hackerNewsRepository = new HackerNewsService(httpClient, mockMemoryCache.Object, mockLogger);

            // Act
            var stories = await hackerNewsRepository.GetNewStoriesAsync();

            // Assert
            Assert.NotNull(stories);
            Assert.Empty(stories);
        }

        [Fact]
        public async Task GetNewStoriesAsync_ShouldReturnCachedStories_WhenDataIsCached()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://hacker-news.firebaseio.com/")
            };

            var mockMemoryCache = new Mock<IMemoryCache>();
            var cacheEntry = new Mock<ICacheEntry>();
            var cachedStories = new List<NewsStory>
    {
        new NewsStory { Id = 1, Title = "Cached Story 1" },
        new NewsStory { Id = 2, Title = "Cached Story 2" }
    };

            object cacheValue = cachedStories;
            mockMemoryCache
                .Setup(mc => mc.TryGetValue(It.IsAny<object>(), out cacheValue))
                .Returns(true);

            var mockLogger = new NullLogger<HackerNewsService>();

            var hackerNewsRepository = new HackerNewsService(httpClient, mockMemoryCache.Object, mockLogger);

            // Act
            var stories = await hackerNewsRepository.GetNewStoriesAsync();

            // Assert
            Assert.NotNull(stories);
            Assert.NotEmpty(stories);
            Assert.Equal(2, stories.Count);
            Assert.Equal("Cached Story 1", stories[0].Title);
            Assert.Equal("Cached Story 2", stories[1].Title);
        }

    }
}
