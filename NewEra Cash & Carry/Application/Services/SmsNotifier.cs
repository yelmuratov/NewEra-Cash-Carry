using NewEra_Cash___Carry.Application.Interfaces.NotifyInterfaces;

namespace NewEra_Cash___Carry.Application.Services
{
    public class SmsNotifier : INotifier
    {
        public void Notify(string message)
        {
            // Simulate sending an SMS
            Console.WriteLine($"SMS sent: {message}");
        }
    }
}
