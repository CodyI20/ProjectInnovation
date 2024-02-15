using UnityEngine;

namespace Photon.Menu
{
  [CreateAssetMenu(menuName ="Photon/Menu/Color Palette")]
  public class PhotonMenuColorPalette : ScriptableObject {
    public PhotonMenuColorMap[] Mapping;
  }
}