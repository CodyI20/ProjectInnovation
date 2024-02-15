using TMPro;
using UnityEngine;

namespace Photon.Menu {
  public class PhotonMenuLoadingUI : PhotonMenuUIScreen {
    [SerializeField] protected TMP_Text _text;

    public void SetStatusText(string text) {
      _text.text = text;
    }

    public virtual async void Disconnect() {
      await Connection.DisconnectAsync(ConnectFailReason.UserRequest);
    }
  }
}
