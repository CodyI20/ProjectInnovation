using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Photon.Menu
{
  public abstract class PhotonMenuConnectionBehaviour : MonoBehaviour, IPhotonMenuConnection {
    
    public IPhotonMenuConnection Connection;

    public string SessionName => Connection.SessionName;

    public int MaxPlayerCount => Connection.MaxPlayerCount;

    public bool IsSessionOwner => Connection.IsSessionOwner;

    public abstract IPhotonMenuConnection Create();

    public Task ConnectAsync(IPhotonMenuConnectArgs connectionArgs)
    {
      if (Connection == null)
      {
        Connection = Create();
      }

      return Connection.ConnectAsync(connectionArgs);
    }

    public Task DisconnectAsync(ConnectFailReason reason)
    {
      if (Connection != null)
      {
        return Connection.DisconnectAsync(reason);
      }

      return Task.CompletedTask;
    }
  }
}
