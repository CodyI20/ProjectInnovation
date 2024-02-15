using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Photon.Menu
{
  [CreateAssetMenu(menuName = "Photon/Menu/Config")]
  public class PhotonMenuConfig : ScriptableObject, IPhotonMenuConfig
#if UNITY_EDITOR
    ,
    ISerializationCallbackReceiver
#endif
  {
    [SerializeField] private int _maxPlayers = 6;
    [SerializeField] private int _partyCodeLength = 8;
    [SerializeField] private List<string> _availableAppVersions;
    [SerializeField] private List<string> _availableRegions;

#if UNITY_EDITOR
    [SerializeField] private List<SceneAsset> _availableScenes = new List<SceneAsset>();
#endif

    [SerializeField] [HideInInspector] private List<string> _scenes;

    [SerializeField] private PhotonMenuMachineGuid _machineGuid;

    public List<string> AvailableAppVersions => _availableAppVersions;

    public List<string> AvailableRegions => _availableRegions;

    public List<string> AvailableScenes => _scenes;

    public int MaxPlayerCount => _maxPlayers;

    public int PartyCodeLength => _partyCodeLength;

    public virtual string MachineGuid => _machineGuid?.Guid;

#if UNITY_EDITOR
    public void OnBeforeSerialize()
    {
      _scenes = new List<string>();
      foreach (var sceneAsset in _availableScenes)
      {
        if (sceneAsset != null)
          _scenes.Add(sceneAsset.name);
      }
    }
    public void OnAfterDeserialize() { }
#endif
  }
}
