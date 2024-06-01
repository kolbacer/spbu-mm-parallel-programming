using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Task5.GraphicChatUI.DataModel;

namespace Task5.GraphicChatUI.Services;

public class NetworkService
{
    public List<NetworkDescription> GetAvailableNetworks()
    {
        var adapters = NetworkInterface.GetAllNetworkInterfaces()
            .OrderBy(x => x.OperationalStatus).ToList();
        
        List<NetworkDescription> networkDescriptions = new List<NetworkDescription>(adapters.Count);
        
        foreach (var adapter in adapters)
        {
            string ipAddresses = String.Join(", ", adapter.GetIPProperties().UnicastAddresses
                .Where(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .Select(ip => ip.Address.ToString()));
            
            networkDescriptions.Add(new NetworkDescription
            {
                Id = adapter.Id,
                Name = adapter.Name,
                Description = adapter.Description,
                NetworkInterfaceType = adapter.NetworkInterfaceType.ToString(),
                PhysicalAddress = adapter.GetPhysicalAddress().ToString(),
                OperationalStatus = adapter.OperationalStatus.ToString(),
                Speed = adapter.Speed.ToString(),
                IpAddresses = ipAddresses
            });
        }
    
        return networkDescriptions;
    }
    
}