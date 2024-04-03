using AutoMapper;
using Mediporta.StackOverflowWebAPI.Entities;
using Mediporta.StackOverflowWebAPI.Models;
using Mediporta.StackOverflowWebAPI.Repositories.Interfaces;
using Mediporta.StackOverflowWebAPI.Services.Interface;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Net.Http.Headers;

namespace Mediporta.StackOverflowWebAPI.Services
{
    public class TagsService : ITagsService
    {
        private readonly string _TagforRedis = "SOTags";
        private readonly ICacheRepository _cacheRepository;
        private readonly IMapper _mapper;
        private readonly ISOApiClient _sOApiClient;

        public TagsService(ICacheRepository cacheRepository, IMapper mapper, ISOApiClient sOApiClient)
        {
            _cacheRepository = cacheRepository;
            _mapper = mapper;
            _sOApiClient = sOApiClient;
        }

        public PagedResult<TagsDto> GetTags(TagQuery query)
        {
            var baseTags = _cacheRepository.GetData<List<TagsEntry>>(_TagforRedis);
            baseTags ??= ReloadTags();

            Expression<Func<TagsEntry, object>> selectedColumn;
            switch (query.SortBy)
            {
                case (SortTagBy.Name):
                    selectedColumn = (c => c.Name);
                    break;
                case (SortTagBy.PercentageUse):
                default:
                    selectedColumn = (c => c.PercentageUse);
                    break;

            }

            baseTags = query.SortDirection == SortDirection.ASC
                ? baseTags.AsQueryable().OrderBy(selectedColumn).ToList()
                : baseTags.AsQueryable().OrderByDescending(selectedColumn).ToList();

            var tags = _mapper.Map<List<TagsDto>>(baseTags)
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            var totalItemsCount = baseTags.Count();

            var result = new PagedResult<TagsDto>(tags, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }

        public List<TagsEntry> ReloadTags()
        {
            var tags = _sOApiClient.DownloadTagsAsync().Result;
            CalculateUsages(tags);
            _cacheRepository.SetData(_TagforRedis, tags);
            return tags;
        }


        public void CalculateUsages(List<TagsEntry> tags)
        {
            var maxUsages = tags.Sum(t => t.Count);
            foreach (var tag in tags)
            {
                tag.PercentageUse = 100 * (double)tag.Count / (double)maxUsages;
            }
        }

    }
}