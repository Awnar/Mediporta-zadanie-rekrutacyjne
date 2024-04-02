using Mediporta.StackOverflowWebAPI.Models;
using Mediporta.StackOverflowWebAPI.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Mediporta.StackOverflowWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ILogger<TagsController> _logger;
        private readonly ITagsService _SOTagService;

        public TagsController(ILogger<TagsController> logger, ITagsService SOTagService)
        {
            _logger = logger;
            _SOTagService = SOTagService;
        }

        [HttpGet]
        public ActionResult<PagedResult<TagsDto>> Get([FromQuery] TagQuery query)
        {
            return Ok(_SOTagService.GetTags(query));
        }

        [HttpPost("reload")]
        public ActionResult ReloadTags()
        {
            _SOTagService.ReloadTags();
            return Ok();
        }
    }
}
