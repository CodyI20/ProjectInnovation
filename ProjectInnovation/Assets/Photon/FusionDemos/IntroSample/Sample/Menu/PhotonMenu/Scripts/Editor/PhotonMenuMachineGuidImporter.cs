using UnityEditor.AssetImporters;
using UnityEngine;

namespace Photon.Menu
{
  [ScriptedImporter(1, "guid")]
  public class PhotonMenuMachineGuidImporter : ScriptedImporter {
    public override void OnImportAsset(AssetImportContext ctx) {
      var mainAsset = ScriptableObject.CreateInstance<PhotonMenuMachineGuid>();
      if (mainAsset != null) {
        mainAsset.Guid = System.Guid.NewGuid().ToString();
        ctx.AddObjectToAsset("root", mainAsset);
      }
    }
  }
}