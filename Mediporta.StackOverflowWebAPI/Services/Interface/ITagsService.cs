using Mediporta.StackOverflowWebAPI.Entities;
using Mediporta.StackOverflowWebAPI.Models;

namespace Mediporta.StackOverflowWebAPI.Services.Interface
{
    public interface ITagsService
    {
        public List<TagsEntry> ReloadTags();
        public PagedResult<TagsDto> GetTags(TagQuery query);
    }
}
