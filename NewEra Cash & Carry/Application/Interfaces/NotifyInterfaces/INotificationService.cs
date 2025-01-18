namespace NewEra_Cash___Carry.Application.Interfaces.NotifyInterfaces
{
    public interface INotificationService
    {
        Task NotifyByEmailAsync(string message);
        Task NotifyBySmsAsync(string message);
    }
}
