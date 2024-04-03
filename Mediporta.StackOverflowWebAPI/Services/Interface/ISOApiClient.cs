using Mediporta.StackOverflowWebAPI.Entities;

namespace Mediporta.StackOverflowWebAPI.Services.Interface
{
    public interface ISOApiClient
    {
        public Task<List<TagsEntry>> DownloadTagsAsync();
    }
}