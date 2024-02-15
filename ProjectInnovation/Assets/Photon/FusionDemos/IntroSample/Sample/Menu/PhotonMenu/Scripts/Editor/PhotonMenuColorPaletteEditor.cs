using UnityEditor;
using UnityEngine;

namespace Photon.Menu
{
  [CustomEditor(typeof(PhotonMenuColorPalette))]
  public class PhotonMenuColorPaletteEditor : UnityEditor.Editor {
    public override void OnInspectorGUI() {
      var data = (PhotonMenuColorPalette)target;

      var map = serializedObject.FindProperty("Mapping");
      for (int i = 0; i < data.Mapping.Length; i++) {
        var prop = map.GetArrayElementAtIndex(i);
        EditorGUILayout.PropertyField(prop, new GUIContent($"Mapping {i:00}"));
      }
      serializedObject.ApplyModifiedProperties();

      if (GUILayout.Button("Reset")) {
        data.Mapping = new PhotonMenuColorMap[0];
        EditorUtility.SetDirty(data);
      }
    }
  }
}
