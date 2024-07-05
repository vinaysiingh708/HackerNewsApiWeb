using HackerNewsApiWeb.Models;
using HackerNewsApiWeb.Repository;
using HackerNewsApiWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("newstories")]
        public async Task<IActionResult> GetNewStories()
        {
            var newStories = await _hackerNewsService.GetNewStoriesAsync();
            if (newStories == null || newStories.Count == 0)
                return NotFound();

            return Ok(newStories);
        }

        
    }
}
