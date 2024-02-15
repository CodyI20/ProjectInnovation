using System;
using System.Collections.Generic;
using Fusion.Sockets;

namespace Fusion.Menu.Helpers {
  public class FusionCallbacks : INetworkRunnerCallbacks {
    public event Action<PlayerRef> OnPlayerJoined;
    public event Action<PlayerRef> OnPlayerLeft;
    public event Action<ShutdownReason> OnShutdown;

    

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { OnPlayerJoined?.Invoke(player); }
    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { OnPlayerLeft?.Invoke(player); }
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
      // Since we have to force autohost to fail if no session exists and handle that when connecting,
      // it shouldn't invoke the callback.
      if (runner.GameMode == GameMode.AutoHostOrClient && shutdownReason == ShutdownReason.GameNotFound)
        return;

      OnShutdown?.Invoke(shutdownReason);
    }

    // Unused callbacks
    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) {}
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) {}
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {}
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) {}
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) {}
  }
}
