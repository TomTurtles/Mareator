namespace Mareator;

public interface INotificationHandler<TNotification> where TNotification : Notification
{
    Task Handle(TNotification notification);
}
