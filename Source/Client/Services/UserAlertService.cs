using GoodsTracker.Platform.Client.Models;

namespace GoodsTracker.Platform.Client.Services;

public sealed class UserAlertService
{
    internal List<UserAlert> RaisedAlerts { get; }
    public event Action? RefreshRequested;

    public UserAlertService()
    {
        this.RaisedAlerts = new List<UserAlert>();
    }

    internal void AddMessage(UserAlert alert)
    {
        this.RaisedAlerts.Add(alert);
        this.RefreshRequested?.Invoke();

        // pop message off after a delay
        _ = new Timer(
            _ =>
            {
                this.RaisedAlerts.RemoveAt(0);
                this.RefreshRequested!.Invoke();
            }, null, 8000, Timeout.Infinite);
    }
}
