using Photon.Menu;
using UnityEngine;
using UnityEngine.Events;

namespace Fusion.Menu
{
  public class FusionConnectionBehaviour : PhotonMenuConnectionBehaviour
  {
    public FusionMenuUIController UIController;
    [Space]
    [Header("Register events to be called when the game starts.")]
    public UnityEvent<NetworkRunner> OnGameStartedCallback;
    [Space]
    [Header("Provide a NetworkRunner prefab to be instantiated. \nIf no prefab is provided, a simple one will be created.")]
    public NetworkRunner NetworkRunnerPrefab;

    public override IPhotonMenuConnection Create()
    {
      return new FusionMenuConnection { UIController = UIController, OnGameStartCallbacks = OnGameStartedCallback, NetworkRunnerPrefab = NetworkRunnerPrefab};
    }
  }
}
