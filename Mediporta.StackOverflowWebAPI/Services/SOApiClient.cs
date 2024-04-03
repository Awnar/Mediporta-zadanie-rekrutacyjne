using Mediporta.StackOverflowWebAPI.Entities;
using Mediporta.StackOverflowWebAPI.Services.Interface;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Net.Http.Headers;

namespace Mediporta.StackOverflowWebAPI.Services
{
    public class SOApiClient : ISOApiClient
    {
        public async Task<List<TagsEntry>> DownloadTagsAsync()
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
    }
}
