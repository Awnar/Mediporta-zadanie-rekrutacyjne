using Mediporta.StackOverflowWebAPI.Entities;

namespace Mediporta.StackOverflowWebAPI.Models
{
    public class TagsDto
    {
        public string Name { get; set; }
        public double PercentageUse { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (TagsDto)obj;
            return Name == other.Name &&
                   PercentageUse == other.PercentageUse;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Name.GetHashCode();
                hash = (hash << 3) + PercentageUse.GetHashCode();
                return hash;
            }
        }
    }
}
