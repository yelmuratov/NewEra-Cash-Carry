using NewEra_Cash___Carry.Application.Interfaces.NotifyInterfaces;

namespace NewEra_Cash___Carry.Application.Services
{
    public class EmailNotifier : INotifier
    {
        public void Notify(string message)
        {
            // Simulate sending an email
            Console.WriteLine($"Email sent: {message}");
        }
    }
}
