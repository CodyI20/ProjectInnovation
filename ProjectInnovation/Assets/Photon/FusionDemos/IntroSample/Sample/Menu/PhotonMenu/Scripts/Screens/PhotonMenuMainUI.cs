using TMPro;
using UnityEngine;

namespace Photon.Menu {
  public class PhotonMenuMainUI : PhotonMenuUIScreen {
    [SerializeField] private TMP_Text _usernamePreview;
    [SerializeField] private TMP_Text _regionAndVersionField;
    [SerializeField] private TMP_InputField _usernameField;
    [SerializeField] private GameObject _usernameView;
    
    public override void Show()
    {
      base.Show();

      ConnectionArgs.SetDefaults(Config);

      _usernameView.SetActive(false);
      _usernamePreview.SetText(ConnectionArgs.Username);

      _regionAndVersionField.SetText(ConnectionArgs.InformationalVersion);
    }

    public void OnFinishUsernameEdit()
    {
      OnFinishUsernameEdit(_usernameField.text);
    }

    public void OnFinishUsernameEdit(string username)
    {
      _usernameView.SetActive(false);

      if (string.IsNullOrEmpty(username) == false)
      {
        _usernamePreview.SetText(username);
        ConnectionArgs.Username = username;
      }
    }

    public void OnSettingsButtonPressed()
    {
      Controller.Show<PhotonMenuSettingsUI>();
    }

    public void OnPartyButtonPressed()
    {
      Controller.Show<PhotonMenuPartyUI>();
    }

    public void OnUsernameButtonPressed()
    {
      _usernameView.SetActive(true);
      _usernameField.text = _usernamePreview.text;
    }

    public async void OnQuickJoinButtonPressed()
    {
      ConnectionArgs.Session = null;
      ConnectionArgs.Creating = false;
      await Connection.ConnectAsync(ConnectionArgs);
    }
  }
}