namespace GoodsTracker.Platform.Client.Models;

public class UserAlert
{
    public UserAlert(string message, Alerts alert)
    {
        this.Message = message;
        this.Alert = alert;
    }

    public string Message { get; init; }
    public Alerts Alert { get; init; }
}

public enum Alerts
{
    Primary,
    Secondary,
    Success,
    Info,
    Danger,
    Warning,
    Dark,
    Light,
}