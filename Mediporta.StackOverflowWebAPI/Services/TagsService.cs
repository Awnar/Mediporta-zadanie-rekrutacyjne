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

        public TagsService(ICacheRepository cacheRepository, IMapper mapper)
        {
            _cacheRepository = cacheRepository;
            _mapper = mapper;
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
            var tags = DownloadTagsAsync().Result;
            CalculateUsages(tags);
            _cacheRepository.SetData(_TagforRedis, tags);
            return tags;
        }

        private async Task<List<TagsEntry>> DownloadTagsAsync()
        {
            List<TagsEntry> results = [];

            string _apiUrl = "https://api.stackexchange.com/2.3/tags?order=desc&sort=popular&site=stackoverflow&pagesize=100&page=";
            int page = 1;

            while (results.Count < 1000)
            {
                string responseBody;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.GetAsync(_apiUrl + (page++));
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        if (response.Content.Headers.ContentEncoding.Any())
                        {
                            using (var decompressedStream = new MemoryStream())
                            {
                                var decompressionStream = new GZipStream(stream, CompressionMode.Decompress);
                                await decompressionStream.CopyToAsync(decompressedStream);
                                decompressionStream.Dispose();
                                decompressedStream.Seek(0, SeekOrigin.Begin);

                                using (var reader = new StreamReader(decompressedStream))
                                {
                                    responseBody = await reader.ReadToEndAsync();
                                }
                            }
                        }
                        else
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                responseBody = await reader.ReadToEndAsync();
                            }
                        }
                    }
                }

                results.AddRange(
                    JsonConvert.DeserializeAnonymousType(responseBody, new
                    {
                        items = new List<TagsEntry>()
                    }).items
                    );
            }

            return results;
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