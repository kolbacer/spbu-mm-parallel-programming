using System;

namespace Task5.GraphicChatUI.DataModel;

public class LocalMessage
{
    public string Sender { get; set; } = String.Empty;
    public string Text { get; set; } = String.Empty;
    public LocalMessageType Type { get; set; }
    public DateTime TimeStamp { get; set; }
    
    public bool IsInfo => Type == LocalMessageType.Info;
    public bool IsYou => Type == LocalMessageType.You;
    public string FormattedTimeStamp => $"[{TimeStamp.ToLocalTime()}]";
}