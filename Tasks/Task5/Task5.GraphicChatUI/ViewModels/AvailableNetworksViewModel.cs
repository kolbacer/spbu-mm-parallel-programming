using System.Collections.ObjectModel;
using Task5.GraphicChatUI.DataModel;
using Task5.GraphicChatUI.Services;

namespace Task5.GraphicChatUI.ViewModels;

public class AvailableNetworksViewModel : ViewModelBase
{
    public AvailableNetworksViewModel()
    {
        var service = new NetworkService();
        AvailableNetworkList = new ObservableCollection<NetworkDescription>(service.GetAvailableNetworks());
    }
    
    public ObservableCollection<NetworkDescription> AvailableNetworkList { get; set; }
}