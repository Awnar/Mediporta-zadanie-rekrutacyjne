using AutoMapper;
using Mediporta.StackOverflowWebAPI.Entities;
using Mediporta.StackOverflowWebAPI.Models;

namespace Mediporta.StackOverflowWebAPI
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<TagsEntry, TagsDto>();
        }
    }
}
