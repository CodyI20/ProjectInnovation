using Photon.Menu;
using UnityEngine;

namespace Fusion.Menu {
  public class FusionGameplayView : PhotonMenuGameplayUI {
    [SerializeField] private FusionPlayersService _playersServicePrefab;
    private FusionPlayersService _playersService;
    [SerializeField] private Camera _menuCamera;
    
    private NetworkRunner _runner;

    public override void Show()
    {
      base.Show();
      if (_runner == null) {
        _runner = FindFirstObjectByType<NetworkRunner>();
      }
      
      if (Connection.IsSessionOwner)
      {
        _playersService = _runner.Spawn(_playersServicePrefab);
      }
      
      _menuCamera.gameObject.SetActive(false);
    }

    public override void Hide() {
      base.Hide();
      _menuCamera.gameObject.SetActive(true);
    }
  }
}
