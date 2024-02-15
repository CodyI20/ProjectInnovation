using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Menu {
  public class PhotonMenuLobbyUI : PhotonMenuUIScreen {
    [SerializeField] protected TMP_Text _sessionField;
    [SerializeField] protected TMP_Text _regionAndVersionField;
    [SerializeField] protected TMP_Text _playersText;
    [SerializeField] protected TMP_Text _playersCountText;
    [SerializeField] protected TMP_Text _playersMaxCountText;
    [SerializeField] protected Button _startGameButton;

    public override void Show()
    {
      base.Show();
      SetSessionInfo();
    }
    
    public void CopySessionGUID()
    {
      GUIUtility.systemCopyBuffer = _sessionField.text;
    }

    public virtual void SetUsernames(List<string> usernames)
    {
      var sBuilder = new StringBuilder();
      foreach (var username in usernames)
      {
        sBuilder.AppendLine(username);
      }
      _playersText.text = sBuilder.ToString();
      
      var maxPlayers = Connection.MaxPlayerCount.ToString();
      var currentPlayersCount = usernames.Count.ToString();
      
      _playersCountText.SetText(currentPlayersCount);
      _playersMaxCountText.SetText($"/{maxPlayers}");
    }

    protected virtual void SetSessionInfo()
    {
      _regionAndVersionField.SetText(ConnectionArgs.InformationalVersion);
      _sessionField.SetText(Connection.SessionName);
      _startGameButton.gameObject.SetActive(Connection.IsSessionOwner);
    }

    public async void DisconnectButtonPressed()
    {
      await Connection.DisconnectAsync(ConnectFailReason.UserRequest);
    }

    //public async void PlayButtonPressed()
    //{
    //  // TODO: this screen probably goes away, otherwise bring start game from inside the room back
    //  await MenuServiceLocator.Connection.StartGameAsync();
    //}
  }
}
