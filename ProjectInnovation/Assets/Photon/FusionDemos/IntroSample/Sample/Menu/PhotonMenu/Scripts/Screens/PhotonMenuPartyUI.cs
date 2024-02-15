using TMPro;
using UnityEngine;

namespace Photon.Menu {
  public partial class PhotonMenuPartyUI : PhotonMenuUIScreen
  {
    [SerializeField] private TMP_InputField _sessionCodeField;
    [SerializeField] private TMP_Text _regionAndVersionField;
  
    public override void Show()
    {
      base.Show();
      var version = ConnectionArgs.InformationalVersion;
      _regionAndVersionField.SetText(version);
    }

    public virtual async void ConnectButton(bool creating)
    {
      ConnectionArgs.Creating = creating;

      if (creating)
      {
        ConnectionArgs.Session = UniqueSessionCodeGenerator.GetRandomCode(Config.PartyCodeLength);
      }
      else 
      {
        var code = _sessionCodeField.text;
        if (UniqueSessionCodeGenerator.IsSessionNameValid(code) == false)
        {
          await Controller.PopupAsync($"Please provide a valid session code {code}", "Invalid Session Code");
          return;
        }

        ConnectionArgs.Session = code;
      }
    
      await Connection.ConnectAsync(ConnectionArgs);
    }
  } 
}