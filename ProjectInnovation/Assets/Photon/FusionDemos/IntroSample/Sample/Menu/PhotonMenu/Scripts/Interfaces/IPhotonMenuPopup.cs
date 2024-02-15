using System.Threading.Tasks;

namespace Photon.Menu
{
  public interface IPhotonMenuPopup
  {
    void OpenPopup(string msg, string header);
    Task OpenPopupAsync(string msg, string header);
  }
}
