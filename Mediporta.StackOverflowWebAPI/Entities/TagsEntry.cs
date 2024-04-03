namespace Mediporta.StackOverflowWebAPI.Entities
{
    public class TagsEntry
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public double PercentageUse { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (TagsEntry)obj;
            return Name == other.Name &&
                   Count == other.Count &&
                   PercentageUse == other.PercentageUse;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Name.GetHashCode();
                hash = (hash << 3) + Count.GetHashCode();
                hash = (hash << 3) + PercentageUse.GetHashCode();
                return hash;
            }
        }
    }
}