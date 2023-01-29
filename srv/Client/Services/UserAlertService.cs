using GoodsTracker.Platform.Client.Models;

namespace GoodsTracker.Platform.Client.Services;

public class UserAlertService
{
    public List<UserAlert> RaisedAlerts { get; private set; }
    public event Action RefreshRequested;

    public UserAlertService()
    {
        RaisedAlerts = new List<UserAlert>();
    }

    public void AddMessage(UserAlert alert)
    {
        RaisedAlerts.Add(alert);
        RefreshRequested?.Invoke();

        // pop message off after a delay
        _ = new Timer((_) =>
        {
            RaisedAlerts.RemoveAt(0);
            RefreshRequested?.Invoke();
        }, null, 8000, Timeout.Infinite);
    }
}