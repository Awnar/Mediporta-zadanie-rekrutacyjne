namespace Mediporta.StackOverflowWebAPI.Repositories.Interfaces
{
    public interface ICacheRepository
    {
        T GetData<T>(string key);
        void SetData<T>(string key, T value);
        void RemoveData(string key);
    }
}
