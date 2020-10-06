namespace EventHubHostedServices.BuildingBlocks.Contracts
{
    public interface ICachingService
    {
        void SetValue<T>(object key, T value);
        T GetValue<T>(object key);
        bool TryGetValue<T>(object key, out T value);
        void RemoveValue(object key);
    }
}
