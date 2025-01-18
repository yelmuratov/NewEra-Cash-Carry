namespace NewEra_Cash___Carry.Application.Interfaces.NotifyInterfaces
{
    public interface INotificationRepository
    {
        Task SaveNotificationAsync(string message, string type);
    }
}
