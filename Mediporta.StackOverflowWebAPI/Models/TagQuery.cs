namespace Mediporta.StackOverflowWebAPI.Models
{
    public class TagQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public SortTagBy SortBy { get; set; } = SortTagBy.PercentageUse;
        public SortDirection SortDirection { get; set; } = SortDirection.DESC;
    }
}
