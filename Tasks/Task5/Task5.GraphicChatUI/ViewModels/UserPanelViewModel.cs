using System;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Task5.GraphicChatUI.Services;
using Task5.GraphicChatUI.Views;

namespace Task5.GraphicChatUI.ViewModels;

public class UserPanelViewModel : ViewModelBase
{
    public ChatPeerService Service { get; }
    
    private string _username = string.Empty;
    private string _ip = string.Empty;
    private string _port = string.Empty;
    
    private string _peerIp = string.Empty;
    private string _peerPort = string.Empty;
    
    private bool _isUserFormEnabled = false;
    private bool _isPeerConnectFormEnabled = false;
    
    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }
    
    public string Ip
    {
        get => _ip;
        set => this.RaiseAndSetIfChanged(ref _ip, value);
    }
    
    public string Port
    {
        get => _port;
        set => this.RaiseAndSetIfChanged(ref _port, value);
    }
    
    public string PeerIp
    {
        get => _peerIp;
        set => this.RaiseAndSetIfChanged(ref _peerIp, value);
    }
    
    public string PeerPort
    {
        get => _peerPort;
        set => this.RaiseAndSetIfChanged(ref _peerPort, value);
    }
    
    public bool IsUserFormEnabled
    {
        get => _isUserFormEnabled;
        set => this.RaiseAndSetIfChanged(ref _isUserFormEnabled, value);
    }
    
    public bool IsPeerConnectFormEnabled
    {
        get => _isPeerConnectFormEnabled;
        set => this.RaiseAndSetIfChanged(ref _isPeerConnectFormEnabled, value);
    }
    
    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }
    public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
    
    public UserPanelViewModel(ChatPeerService service)
    {
        Service = service;

        var isValidUserInput = this.WhenAnyValue(
                x => x.Username,
                x => x.Ip,
                x => x.Port)
            .Select(props =>
                !string.IsNullOrWhiteSpace(props.Item1) &&
                !string.IsNullOrWhiteSpace(props.Item2) &&
                !string.IsNullOrWhiteSpace(props.Item3));
        
        var isValidPeerConnectInput = this.WhenAnyValue(
                x => x.PeerIp,
                x => x.PeerPort)
            .Select(props =>
                !string.IsNullOrWhiteSpace(props.Item1) &&
                !string.IsNullOrWhiteSpace(props.Item2));

        var isValidStart = isValidUserInput.CombineLatest(
            Service.IsRunningObservable.Select(x => !x),
            (a, b) => a && b).DistinctUntilChanged();

        var isValidStop = Service.IsRunningObservable;
        
        var isValidConnect = isValidPeerConnectInput.CombineLatest(
            Service.IsRunningObservable,
            (a, b) => a && b).DistinctUntilChanged();

        Service.IsRunningObservable.Subscribe(x =>
        {
            IsUserFormEnabled = !x;
            IsPeerConnectFormEnabled = x;
        });

        StartCommand = ReactiveCommand.Create(StartPeer, isValidStart);
        StopCommand = ReactiveCommand.Create(StopPeer, isValidStop);
        ConnectCommand = ReactiveCommand.Create(ConnectPeer, isValidConnect);
    }

    public void StartPeer()
    {
        Service.Start(Username, new IPEndPoint(IPAddress.Parse(Ip),  int.Parse(Port)));
    }
    
    public void StopPeer()
    {
        Service.Stop();
    }
    
    public void ConnectPeer()
    {
        Service.Connect(PeerIp, int.Parse(PeerPort));
    }

    public void ShowAvailableNetworks()
    {
        var viewModel = new AvailableNetworksViewModel();
        var availableNetworksWindow = new AvailableNetworksWindow(viewModel);
        availableNetworksWindow.Show();
    }
}