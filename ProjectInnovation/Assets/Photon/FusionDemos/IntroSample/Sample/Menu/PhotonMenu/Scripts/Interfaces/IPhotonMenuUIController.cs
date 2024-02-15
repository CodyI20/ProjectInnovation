using System.Threading.Tasks;

namespace Photon.Menu {  
  public interface IPhotonMenuUIController {
    void Show<S>() where S : PhotonMenuUIScreen;
    void Popup(string msg, string header = default);
    public Task PopupAsync(string msg, string header = default);
  }
}
