using System.Threading.Tasks;

namespace Photon.Menu {
  public interface IPhotonMenuConnection
  {
    string SessionName { get; }
    int MaxPlayerCount { get; }
    // Region
    // Cloud
    // AppVersion
    bool IsSessionOwner { get; }

    Task ConnectAsync(IPhotonMenuConnectArgs connectArgs);
    Task DisconnectAsync(ConnectFailReason reason);
  }
}
