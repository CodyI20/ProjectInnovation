using System.Collections.Generic;

namespace Photon.Menu
{
  public interface IPhotonMenuConfig
  {
    List<string> AvailableAppVersions { get; }
    List<string> AvailableRegions { get; }
    List<string> AvailableScenes { get; }
    int MaxPlayerCount { get; }
    int PartyCodeLength { get; }
    string MachineGuid { get; } 
  }
}
