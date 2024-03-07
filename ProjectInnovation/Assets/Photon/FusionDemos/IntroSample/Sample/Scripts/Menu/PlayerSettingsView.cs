using System;
using System.Collections.Generic;
using Fusion;
using Photon.Menu;
using UnityEngine;

namespace FusionUtils {
  public class PlayerSettingsView : PhotonMenuUIScreen {
    // List of avatars
    [SerializeField] private List<NetworkPrefabRef> _availableHostModeAvatars;
    [SerializeField] private List<NetworkPrefabRef> _availableSharedModeAvatars;
    [SerializeField] private Transform _avatarHolder;
    [SerializeField] private NetworkProjectConfigAsset _networkProjectConfig;

    private GameObject _currentAvatarModel;
    private Quaternion _prevAvatarRotation;
    private List<NetworkPrefabRef> _availableAvatars;

    // Current selected avatar
    private int _currentIndex;

    private NetworkProjectConfigAsset NetworkProjectConfig {
      get {
        if (_networkProjectConfig == null) {
          _networkProjectConfig = NetworkProjectConfigAsset.Global;
        }

        return _networkProjectConfig;
      }
      set => _networkProjectConfig = value;
    }

    public override void Show() {
      base.Show();
      _availableAvatars = _availableSharedModeAvatars;
      RenderAvatar();
    }

    public override void Hide() {
      base.Hide();
      if (_currentAvatarModel)
        Destroy(_currentAvatarModel);
    }

    public void NextAvatar() {
      _currentIndex = (_currentIndex + 1) % _availableAvatars.Count;
      RenderAvatar();
    }

    public void PreviousAvatar() {
      if (_currentIndex <= 0) {
        _currentIndex = _availableAvatars.Count - 1;
      } else {
        _currentIndex = (_currentIndex - 1) % _availableAvatars.Count;
      }

      RenderAvatar();
    }

    public NetworkPrefabId GetSelectedSkin(Topologies topology) {
      _availableAvatars = topology == Topologies.Shared ? _availableSharedModeAvatars : _availableHostModeAvatars;
      return NetworkProjectConfig.Config.PrefabTable.GetId((NetworkObjectGuid)_availableAvatars[_currentIndex]);
    }

    private void RenderAvatar() {
      if (_currentAvatarModel) {
        //_prevAvatarRotation = _currentAvatarModel.transform.rotation;
        Destroy(_currentAvatarModel);
      }

      var prefabRef = _availableAvatars[_currentIndex];
      if (!prefabRef.IsValid) {
        throw new ArgumentException($"Not valid.", nameof(prefabRef));
      }

      var model = NetworkProjectConfig.Config.PrefabTable.Load(NetworkProjectConfig.Config.PrefabTable.GetId((NetworkObjectGuid)prefabRef), true);
      _currentAvatarModel = Instantiate(model.gameObject, _avatarHolder);
            _currentAvatarModel.transform.position += new Vector3(-20f, 0, 0);
      //_currentAvatarModel.AddComponent<RotateAvatar>();
      //_currentAvatarModel.transform.rotation = _prevAvatarRotation;
    }
  }
}