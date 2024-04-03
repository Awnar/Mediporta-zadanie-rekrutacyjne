using NUnit.Framework;
using Mediporta.StackOverflowWebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Mediporta.StackOverflowWebAPI.Repositories.Interfaces;
using Moq;
using Mediporta.StackOverflowWebAPI.Entities;
using Mediporta.StackOverflowWebAPI.Models;
using Mediporta.StackOverflowWebAPI.Services.Interface;
using Assert = NUnit.Framework.Assert;

namespace Mediporta.StackOverflowWebAPI.Services.Tests
{
    [TestFixture()]
    public class TagsServiceTests
    {
        private Mock<ICacheRepository> _cacheRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISOApiClient> _soApiClientMock;
        private TagsService _tagsService;

        [SetUp]
        public void Setup()
        {
            _cacheRepositoryMock = new Mock<ICacheRepository>();
            _mapperMock = new Mock<IMapper>();
            _soApiClientMock = new Mock<ISOApiClient>();
            _tagsService = new TagsService(_cacheRepositoryMock.Object, _mapperMock.Object, _soApiClientMock.Object);
        }

        [Test]
        public void GetTags_DataFromCache_ReturnsCorrectResult()
        {
            // Arrange
            var query = new TagQuery { SortBy = SortTagBy.Name, SortDirection = SortDirection.ASC, PageSize = 10, PageNumber = 1 };
            var tagsEntryList = new List<TagsEntry>
            {
                new() { Name = "Tag1" },
                new() { Name = "Tag2" },
                new() { Name = "Tag3" }
            };
            var tagsDtoList = new List<TagsDto>
            {
                new() { Name = "Tag1" },
                new() { Name = "Tag2" },
                new() { Name = "Tag3" }
            };

            _cacheRepositoryMock.Setup(x => x.GetData<List<TagsEntry>>(It.IsAny<string>())).Returns(tagsEntryList);
            _mapperMock.Setup(x => x.Map<List<TagsDto>>(tagsEntryList)).Returns(tagsDtoList);

            // Act
            var result = _tagsService.GetTags(query);

            // Assert
            Assert.NotNull(result);
            Assert.IsNotEmpty(result.Items);
            Assert.AreEqual(tagsDtoList, result.Items);
            _cacheRepositoryMock.Verify(x => x.GetData<List<TagsEntry>>(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void GetTags_DataFromSOApi_ReturnsCorrectResult()
        {
            // Arrange
            var query = new TagQuery { SortBy = SortTagBy.Name, SortDirection = SortDirection.ASC, PageSize = 10, PageNumber = 1 };
            var tagsEntryList = new List<TagsEntry>
            {
                new() { Name = "Tag1", Count = 20 },
                new() { Name = "Tag2", Count = 30 },
                new() { Name = "Tag3", Count = 50 }
            };
            var tagsDtoList = new List<TagsDto>
            {
                new() { Name = "Tag1", PercentageUse = 20 },
                new() { Name = "Tag2", PercentageUse = 30 },
                new() { Name = "Tag3", PercentageUse = 50 }
            };

            _cacheRepositoryMock.Setup(x => x.GetData<List<TagsEntry>>(It.IsAny<string>())).Returns((List<TagsEntry>)null);
            _mapperMock.Setup(x => x.Map<List<TagsDto>>(tagsEntryList)).Returns(tagsDtoList);
            _soApiClientMock.Setup(x => x.DownloadTagsAsync()).ReturnsAsync(tagsEntryList);

            // Act
            var result = _tagsService.GetTags(query);

            // Assert
            Assert.NotNull(result);
            Assert.IsNotEmpty(result.Items);
            Assert.AreEqual(tagsDtoList, result.Items);
            _cacheRepositoryMock.Verify(x => x.GetData<List<TagsEntry>>(It.IsAny<string>()), Times.Once());
            _soApiClientMock.Verify(x => x.DownloadTagsAsync(), Times.Once());
        }

        [Test]
        public void ReloadTags_ReturnsCorrectResult_ReturnsCorrectResult()
        {
            // Arrange
            var tagsEntryList = new List<TagsEntry>
                {
                new() { Name = "Tag1", Count = 20 },
                new() { Name = "Tag2", Count = 30 },
                new() { Name = "Tag3", Count = 50 }
            };
            var tagsOutList = new List<TagsEntry>
                {
                new() { Name = "Tag1", Count = 20, PercentageUse = 20 },
                new() { Name = "Tag2", Count = 30, PercentageUse = 30 },
                new() { Name = "Tag3", Count = 50, PercentageUse = 50 }
            };

            _cacheRepositoryMock.Setup(x => x.SetData(It.IsAny<string>(), It.IsAny<object>()));
            _soApiClientMock.Setup(x => x.DownloadTagsAsync()).ReturnsAsync(tagsEntryList);

            // Act
            var result = _tagsService.ReloadTags();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(tagsOutList, result);
            _cacheRepositoryMock.Verify(x => x.SetData(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
            _soApiClientMock.Verify(x => x.DownloadTagsAsync(), Times.Once());
        }

        [Test()]
        public void CalculateUsages_WithNegativeNumbers_ThrowException()
        {
            // Arrange
            var tagsEntryList = new List<TagsEntry>
                {
                new() { Name = "Tag1", Count = -20 },
                new() { Name = "Tag2", Count = -30 },
                new() { Name = "Tag3", Count = 50 }
            };

            // Act
            // Assert
            Assert.Catch<ArgumentException>( () => _tagsService.CalculateUsages(tagsEntryList) );
        }
    }
}