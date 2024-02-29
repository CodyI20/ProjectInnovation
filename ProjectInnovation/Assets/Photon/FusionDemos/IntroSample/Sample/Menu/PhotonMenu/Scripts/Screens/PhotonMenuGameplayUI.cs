using System;
using System.Collections.Generic;
using System.Text;
using Fusion;
using TMPro;
using UnityEngine;

namespace Photon.Menu {
  public class PhotonMenuGameplayUI : PhotonMenuUIScreen {

    public static event Action OnPlayerConnect;

    [SerializeField] private TMP_Text _codeText;
    [SerializeField] private TMP_Text _informationalVersion;
    [SerializeField] protected TMP_Text _playersText;
    [SerializeField] protected TMP_Text _playersCountText;
    [SerializeField] protected TMP_Text _playersMaxCountText;

    public void CopySessionGUID()
    {
      GUIUtility.systemCopyBuffer = _codeText.text;
    }

    public async void Disconnect()
    {
      await Connection.DisconnectAsync(ConnectFailReason.UserRequest);
      Controller.Show<PhotonMenuMainUI>();
    }

    public void RetractOverlay()
    {
      _animator.Play(HideAnimHash);
    }

    public void ExpandOverlay()
    {
      _animator.Play(ShowAnimHash);
    }

    public override void Show()
    {
      base.Show();
      _codeText.SetText(Connection.SessionName);
      var runner = FindFirstObjectByType<NetworkRunner>();
      if (runner != null && runner.IsRunning) {
        _informationalVersion.SetText($"{runner.SessionInfo.Region} | {ConnectionArgs.AppVersion}");
      } else {
        _informationalVersion.SetText(ConnectionArgs.InformationalVersion);
      }
      
      ExpandOverlay();
    }

    public void SetUsernames(List<string> usernames)
    {
      var sBuilder = new StringBuilder();
      foreach (var username in usernames)
      {
        sBuilder.AppendLine(username);
      }
      _playersText.text = sBuilder.ToString();
      
      var maxPlayers = Connection.MaxPlayerCount;
      var currentPlayersCount = usernames.Count.ToString();

            OnPlayerConnect?.Invoke();
      
      _playersCountText.SetText(currentPlayersCount);
      _playersMaxCountText.SetText($"/{maxPlayers}");
    }
  }
}
