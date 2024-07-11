using HackerNewsApiWeb.Models;
using HackerNewsApiWeb.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;


namespace HackerNewsApiWeb.Tests
{
    public class HackerNewsRepositoryTests
    {
        [Fact]
        public async Task GetNewStoriesAsync_ShouldReturnStories_WhenApiReturnsData()
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
                            Content = new StringContent(JsonSerializer.Serialize(new List<int> { 1, 2, 3 }))
                        };
                    }
                    else if (request.RequestUri.AbsolutePath.StartsWith("/v0/item/"))
                    {
                        var idString = request.RequestUri.AbsolutePath.Split('/').Last();
                        var id = int.Parse(idString.Replace(".json", ""));
                        var story = new NewsStory { Id = id, Title = $"Story {id}" };
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(JsonSerializer.Serialize(story))
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

            var mockLogger = new NullLogger<HackerNewsRepository>();

            var hackerNewsRepository = new HackerNewsRepository(httpClient, mockMemoryCache.Object, mockLogger);

            // Act
            var stories = await hackerNewsRepository.GetNewStoriesAsync();

            // Assert
            Assert.NotNull(stories);
            Assert.NotEmpty(stories);
            Assert.Equal(3, stories.Count);
        }
    }
}
