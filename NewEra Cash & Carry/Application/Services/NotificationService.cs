using NewEra_Cash___Carry.Application.Interfaces.NotifyInterfaces;

namespace NewEra_Cash___Carry.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotifier _emailNotifier;
        private readonly INotifier _smsNotifier;

        public NotificationService(INotificationRepository notificationRepository, INotifier emailNotifier, INotifier smsNotifier)
        {
            _notificationRepository = notificationRepository;
            _emailNotifier = emailNotifier;
            _smsNotifier = smsNotifier;
        }

        public async Task NotifyByEmailAsync(string message)
        {
            _emailNotifier.Notify(message);
            await _notificationRepository.SaveNotificationAsync(message, "Email");
        }

        public async Task NotifyBySmsAsync(string message)
        {
            _smsNotifier.Notify(message);
            await _notificationRepository.SaveNotificationAsync(message, "SMS");
        }
    }
}
