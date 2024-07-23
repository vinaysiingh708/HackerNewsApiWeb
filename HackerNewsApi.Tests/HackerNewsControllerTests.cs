using Moq;
using Microsoft.AspNetCore.Mvc;
using HackerNewsApiWeb.Controllers;
using HackerNewsApi.Data.Models;
using Repository.Service;

namespace HackerNewsApi.Tests
{
    public class HackerNewsControllerTests
    {
        private readonly Mock<IHackerNewsService> _mockHackerNewsService;
        private readonly HackerNewsController _controller;

        public HackerNewsControllerTests()
        {
            _mockHackerNewsService = new Mock<IHackerNewsService>();
            _controller = new HackerNewsController(_mockHackerNewsService.Object);
        }

        [Fact]
        public async Task GetNewStories_ReturnsOkResult_WithListOfStories()
        {
            // Arrange
            var stories = new List<NewsStory>
        {
            new NewsStory { Id = 1, Title = "Story 1", Url = "http://abc.com/1" },
            new NewsStory { Id = 2, Title = "Story 2", Url = "http://xyz.com/2" }
        };

            _mockHackerNewsService.Setup(service => service.GetNewStoriesAsync())
                .ReturnsAsync(stories);

            // Act
            var result = await _controller.GetNewStories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<NewsStory>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetNewStories_ReturnsNotFound_WhenNoStories()
        {
            // Arrange
            _mockHackerNewsService.Setup(service => service.GetNewStoriesAsync())
                .ReturnsAsync(new List<NewsStory>());

            // Act
            var result = await _controller.GetNewStories();

            // Assert
            var statusCodeResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, statusCodeResult.StatusCode);           
        }

        [Fact]
        public async Task GetNewStories_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockHackerNewsService.Setup(service => service.GetNewStoriesAsync())
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetNewStories();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);
        }

    }
}