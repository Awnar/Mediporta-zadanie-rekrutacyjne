using Mediporta.StackOverflowWebAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Newtonsoft.Json;
using System.Text;

namespace Mediporta.StackOverflowWebAPI.IntegrationTests
{
    [TestFixture()]
    public class TagsControllerTest
    {
        private Mock<IDistributedCache> _distributedCache = new();
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton<IDistributedCache>(_distributedCache.Object);
                    });
                })
                .CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Get_WithQueryParameters_ReturnsResult()
        {
            // Arrange
            var request = "/Tags?PageNumber=1&PageSize=2&SortBy=PercentageUse&SortDirection=DESC";
            var tagsList = "[\r\n    {\r\n        \"count\": 2529114,\r\n      \t\"name\": \"javascript\",\r\n\t\t\"percentageUse\": 4.7646411826014266\r\n    },\r\n    {\r\n        \"count\": 2192684,\r\n        \"name\": \"python\",\r\n\t\t\"percentageUse\": 4.130608110967346\r\n\r\n    },\r\n    {\r\n        \"count\": 1917423,\r\n      \t\"name\": \"java\",\r\n\t\t\"percentageUse\": 3.6123921163673076\r\n    },\r\n    {\r\n        \"count\": 1615209,\r\n      \t\"name\": \"c#\",\r\n\t\t\"percentageUse\": 3.0429457506714246\r\n    }\r\n]";
            var tagBytes = Encoding.UTF8.GetBytes(tagsList);
            _distributedCache.Setup(x => x.Get(It.IsAny<string>())).Returns(tagBytes);

            var tagsExpectedResults = new List<TagsDto>()
            {
                new TagsDto()
                {
                    Name = "javascript",
                    PercentageUse = 4.7646411826014266
                },
                new TagsDto()
                {
                    Name = "python",
                    PercentageUse = 4.130608110967346
                }
            };

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            Assert.IsNotNull(response);
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagedResult<TagsDto>>(stringResponse);

            Assert.AreEqual(tagsExpectedResults, result.Items);
        }

        [Test]
        public async Task Get_WithoutQueryParameters_ReturnsResult()
        {
            // Arrange
            var request = "/Tags";
            var tagsList = "[\r\n    {\r\n        \"count\": 2529114,\r\n      \t\"name\": \"javascript\",\r\n\t\t\"percentageUse\": 4.7646411826014266\r\n    },\r\n    {\r\n        \"count\": 2192684,\r\n        \"name\": \"python\",\r\n\t\t\"percentageUse\": 4.130608110967346\r\n\r\n    },\r\n    {\r\n        \"count\": 1917423,\r\n      \t\"name\": \"java\",\r\n\t\t\"percentageUse\": 3.6123921163673076\r\n    },\r\n    {\r\n        \"count\": 1615209,\r\n      \t\"name\": \"c#\",\r\n\t\t\"percentageUse\": 3.0429457506714246\r\n    }\r\n]";
            var tagBytes = Encoding.UTF8.GetBytes(tagsList);
            _distributedCache.Setup(x => x.Get(It.IsAny<string>())).Returns(tagBytes);


            var tagsExpectedResults = new List<TagsDto>()
            {
                new TagsDto()
                {
                    Name = "javascript",
                    PercentageUse = 4.7646411826014266
                },
                new TagsDto()
                {
                    Name = "python",
                    PercentageUse = 4.130608110967346
                },
                new TagsDto()
                {
                    Name = "java",
                    PercentageUse = 3.6123921163673076
                },
                new TagsDto()
                {
                    Name = "c#",
                    PercentageUse = 3.0429457506714246
                },
            };

            // Act
            var response = await _client.GetAsync(request);

            // Assert
            Assert.IsNotNull(response);
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagedResult<TagsDto>>(stringResponse);

            Assert.AreEqual(tagsExpectedResults, result.Items);
        }

        [Test]
        public async Task ReloadTags_ReturnsCorrectResponse()
        {
            // Arrange
            var request = "/tags/reload";

            // Act
            var response = await _client.PostAsync(request, null);

            // Assert
            Assert.IsNotNull(response);
            response.EnsureSuccessStatusCode();
        }
    }
}