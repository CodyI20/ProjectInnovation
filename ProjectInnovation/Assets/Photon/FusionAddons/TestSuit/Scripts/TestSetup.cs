namespace Fusion.Addons.TestSuit {
  
  /// <summary>
  /// Used to describe a single Test, with type of Server and number of connected Clients
  /// </summary>
  public class TestSetup {
    /// <summary>
    /// Server <see cref="GameMode"/>
    /// </summary>
    public GameMode ServerMode;
    
    /// <summary>
    /// Number of client that should connect to the Server
    /// </summary>
    public int ClientCount;
    
    /// <summary>
    /// Fixed Session Name
    /// </summary>
    public string SessionName;

    /// <summary>
    /// Total number of peers of this <see cref="TestSetup"/>
    /// </summary>
    public int TotalPeers => (ServerMode == GameMode.Single || ServerMode == GameMode.Server || ServerMode == GameMode.Host ? 1 : 0) + ClientCount;
    
    /// <summary>
    /// Total number of players of this <see cref="TestSetup"/>
    /// </summary>
    public int TotalPlayers => (ServerMode == GameMode.Host || ServerMode == GameMode.Single ? 1 : 0) + ClientCount;

    public override string ToString() => $"[{nameof(TestSetup)}: {nameof(ServerMode)}={ServerMode}, {nameof(ClientCount)}={ClientCount}]";
  }
}