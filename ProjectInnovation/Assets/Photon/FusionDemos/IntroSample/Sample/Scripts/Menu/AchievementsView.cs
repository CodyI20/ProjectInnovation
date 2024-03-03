using System;
using System.Collections.Generic;
using Fusion;
using Photon.Menu;
using UnityEngine;

namespace FusionUtils
{
    public class AchievementsView : PhotonMenuUIScreen
    {
        // List of models
        [SerializeField] private List<NetworkPrefabRef> _achievementsModels;
        [SerializeField] private Transform _achievementHolder;
        [SerializeField] private NetworkProjectConfigAsset _networkProjectConfig;

        private GameObject _currentAchievementModel;
        private Quaternion _prevModelRotation;
        private List<NetworkPrefabRef> _avaliableModels;

        // Current selected achievement model
        private int _currentIndex;

        private NetworkProjectConfigAsset NetworkProjectConfig
        {
            get
            {
                if (_networkProjectConfig == null)
                {
                    _networkProjectConfig = NetworkProjectConfigAsset.Global;
                }

                return _networkProjectConfig;
            }
            set => _networkProjectConfig = value;
        }

        public override void Show()
        {
            base.Show();
            _avaliableModels = _achievementsModels;
            RenderModel();
        }

        public override void Hide()
        {
            base.Hide();
            if (_currentAchievementModel)
                Destroy(_currentAchievementModel);
        }

        public void NextModel()
        {
            _currentIndex = (_currentIndex + 1) % _avaliableModels.Count;
            RenderModel();
        }

        public void PreviousModel()
        {
            if (_currentIndex <= 0)
            {
                _currentIndex = _avaliableModels.Count - 1;
            }
            else
            {
                _currentIndex = (_currentIndex - 1) % _avaliableModels.Count;
            }

            RenderModel();
        }

        public NetworkPrefabId GetSelectedSkin(Topologies topology)
        {
            _avaliableModels = _achievementsModels;
            return NetworkProjectConfig.Config.PrefabTable.GetId((NetworkObjectGuid)_avaliableModels[_currentIndex]);
        }

        private void RenderModel()
        {
            if (_currentAchievementModel)
            {
                _prevModelRotation = _currentAchievementModel.transform.rotation;
                Destroy(_currentAchievementModel);
            }

            var prefabRef = _avaliableModels[_currentIndex];
            if (!prefabRef.IsValid)
            {
                throw new ArgumentException($"Not valid.", nameof(prefabRef));
            }

            var model = NetworkProjectConfig.Config.PrefabTable.Load(NetworkProjectConfig.Config.PrefabTable.GetId((NetworkObjectGuid)prefabRef), true);
            _currentAchievementModel = Instantiate(model.gameObject, _achievementHolder);
            _currentAchievementModel.AddComponent<RotateAvatar>();
            _currentAchievementModel.transform.rotation = _prevModelRotation;
        }
    }
}