using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Photon.Menu {
  public class PhotonMenuPopupUI : PhotonMenuUIScreen, IPhotonMenuPopup
  {
    [SerializeField] protected TMP_Text _text;
    [SerializeField] protected TMP_Text _header;

    protected TaskCompletionSource<bool> _taskCompletionSource;

    private new void Awake()
    {
      IsModal = true;
    }

    public override void Hide()
    {
      base.Hide();

      _taskCompletionSource?.SetResult(true);
      _taskCompletionSource = null;
    }

    public void OpenPopup(string msg, string header)
    {
      Show();
      _header.text = header;
      _text.text = msg;
    }

    public Task OpenPopupAsync(string msg, string header)
    {
      _taskCompletionSource?.SetResult(true);
      _taskCompletionSource = new TaskCompletionSource<bool>();

      Show();
      _header.text = header;
      _text.text = msg;

      return _taskCompletionSource.Task;
    }
  }
}
