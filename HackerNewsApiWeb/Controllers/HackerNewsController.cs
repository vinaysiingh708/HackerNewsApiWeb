using Microsoft.AspNetCore.Mvc;
using Repository.Service;
//using Repository.Service;

namespace HackerNewsApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly IHackerNewsService _hackerNewsService;

        public HackerNewsController(IHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        /// <summary>
        /// API to get new stories
        /// </summary>
        /// <returns></returns>
        [HttpGet("newstories")]
        public async Task<IActionResult> GetNewStories()
        {
            try
            {
                var newStories = await _hackerNewsService.GetNewStoriesAsync();
                if (newStories == null || newStories.Count == 0)
                    return NotFound();

                return Ok(newStories);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here if needed
                return StatusCode(500, "Internal server error");
            }
        }        
    }
}
