using System;

namespace Task5.GraphicChatUI.DataModel;

public class NetworkDescription
{
    public string Id { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public string NetworkInterfaceType { get; set; } = String.Empty;
    public string PhysicalAddress { get; set; } = String.Empty;
    public string OperationalStatus { get; set; } = String.Empty;
    public string Speed { get; set; } = String.Empty;
    public string IpAddresses { get; set; } = String.Empty;

    public bool IsUp => OperationalStatus == "Up";
    public bool IsDown => OperationalStatus == "Down";
}