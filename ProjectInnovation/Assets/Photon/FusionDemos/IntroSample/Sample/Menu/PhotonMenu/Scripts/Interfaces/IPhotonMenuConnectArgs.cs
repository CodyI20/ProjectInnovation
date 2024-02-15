namespace Photon.Menu {
  public interface IPhotonMenuConnectArgs
  {
    string Username { get; set; }
    string Session { get; set; }
    string Region { get; set; }
    string AppVersion { get; set; }
    int MaxPlayerCount { get; set; }
    bool Creating { get; set; }

    void SetDefaults(IPhotonMenuConfig config);
    string InformationalVersion { get; } 
  }
}
