using System.Collections.Generic;
using Photon.Menu;
using UnityEngine;

namespace Fusion.Menu {
  [CreateAssetMenu(menuName = "Fusion/Menu Config")]
  public class FusionMenuConfig : PhotonMenuConfig {
    
    [SerializeField] private List<FusionGameMode> _gameModes;

    public List<FusionGameMode> AvailableGameModes => _gameModes;
    
    public FusionGameMode GetGameMode(int index)
    {
      return _gameModes[index];
    }
  }
}
