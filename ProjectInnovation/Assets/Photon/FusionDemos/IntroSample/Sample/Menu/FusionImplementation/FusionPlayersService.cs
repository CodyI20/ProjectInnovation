using Fusion.Menu.Helpers;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Menu
{
    public class FusionPlayersService : NetworkBehaviour
    {
        private const int PLAYERS_MAX_COUNT = 20; // this value needs to be bigger than the one on the config file.

        [Networked, Capacity(PLAYERS_MAX_COUNT)]
        private NetworkDictionary<PlayerRef, NetworkString<_16>> _playersUsernames => default;
        private readonly FusionCallbacks _callbacks = new FusionCallbacks();

        private ChangeDetector _changeDetector;
        private FusionGameplayView _fusionGameplayView;

        public List<string> GetPlayersUsernames()
        {
            var playersList = new List<string>();
            foreach (var pair in _playersUsernames)
            {
                playersList.Add(pair.Value.Value);
            }
            return playersList;
        }

        public override void Spawned()
        {
            _fusionGameplayView = FindFirstObjectByType<FusionGameplayView>(FindObjectsInactive.Include);

            CheckMaxPlayerCount();

            Runner.AddCallbacks(_callbacks);

            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            RPC_AddPlayer(Runner.LocalPlayer, PlayerPrefs.GetString("Photon.Menu.Username"));
        }

        private void OnPlayersChange()
        {
            _fusionGameplayView.SetUsernames(GetPlayersUsernames());
        }

        /// <summary>
        /// Check if the current config max players match the username dictionary capacity.
        /// </summary>
        /// <returns></returns>
        public void CheckMaxPlayerCount()
        {
            if (Runner.SessionInfo.MaxPlayers > PLAYERS_MAX_COUNT)
            {
                Debug.LogWarning($"Current gameplay overlay max clients capacity ({PLAYERS_MAX_COUNT}) is less than the session max players ({Runner.SessionInfo.MaxPlayers}). Consider increasing.");
            }
        }

        public override void Render()
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                if (change == nameof(_playersUsernames))
                {
                    OnPlayersChange();
                }
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_AddPlayer(PlayerRef player, string username)
        {
            _playersUsernames.Add(player, username);
        }

        private void RemovePlayer(PlayerRef player)
        {
            _playersUsernames.Remove(player);
        }

        private void Shutdown(ShutdownReason reason)
        {
            _fusionGameplayView.Disconnect();
        }

        private void OnEnable()
        {
            _callbacks.OnPlayerLeft += RemovePlayer;
            _callbacks.OnShutdown += Shutdown;
        }

        private void OnDisable()
        {
            _callbacks.OnPlayerLeft -= RemovePlayer;
            _callbacks.OnShutdown -= Shutdown;
        }
    }
}
