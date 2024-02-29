using Fusion;
using Photon.Menu;
using UnityEngine;

namespace FusionUtils
{
    /// <summary>
    /// Class responsible to spawn the player avatar with the requested skin for the local player on shared mode sessions.
    /// </summary>
    public class FusionDemoSpawnerShared : NetworkBehaviour
    {
        [Header("Base avatar used if the selected skin avatar from the menu was not found.")]
        [SerializeField] private NetworkPrefabRef _baseAvatar;

        [SerializeField] private Transform[] _spawnPoints;

        private int playersInGame = 1;

        private void OnEnable()
        {
            PhotonMenuGameplayUI.OnPlayerConnect += IncrementPlayersInGame;
        }

        private void OnDisable()
        {
            PhotonMenuGameplayUI.OnPlayerConnect -= IncrementPlayersInGame;
        }

        public override void Spawned()
        {
            // Request server to spawn the selected skin
            var playerSettingsView = FindFirstObjectByType<PlayerSettingsView>(FindObjectsInactive.Include);

            // If the PlayerSettingsView was found, use it to send the RPC with the selected avatar skin and username defined on the menu.
            // Else: Send the RPC with the default base avatar and the local player ref as the player username.
            if (playerSettingsView)
            {
                SpawnPlayerPrefab(playerSettingsView.GetSelectedSkin(Runner.Topology), playerSettingsView.ConnectionArgs.Username);
            }
            else
            {
                SpawnPlayerPrefab(default, Runner.LocalPlayer.ToString());
            }
        }

        // Spawns the player avatar with the specified skin and username.
        private void SpawnPlayerPrefab(NetworkPrefabId avatarSkin, string username)
        {
            Vector3 currentPosition = _spawnPoints[playersInGame - 1].position;
            NetworkObject avatar;

            // Check if the avatar skin is not the default one
            if (avatarSkin != default)
            {
                // Spawn the avatar with the specified skin, position, and username
                avatar = Runner.Spawn(avatarSkin, position: currentPosition, onBeforeSpawned: OnBeforeSpawned);
            }
            else
            {
                // Log a warning and spawn the base avatar if the specified skin was not found
                Debug.LogWarning("Skin not found. Using base avatar");
                avatar = Runner.Spawn(_baseAvatar, position: currentPosition, onBeforeSpawned: OnBeforeSpawned);
            }

            void OnBeforeSpawned(NetworkRunner runner, NetworkObject o)
            {
                o.GetBehaviour<PlayerUsernameLabel>().SetUsernameLabel(username);
            }

            // register the player object
            Runner.SetPlayerObject(Runner.LocalPlayer, avatar);
        }

        private void IncrementPlayersInGame()
        {
            playersInGame++;
        }
    }
}