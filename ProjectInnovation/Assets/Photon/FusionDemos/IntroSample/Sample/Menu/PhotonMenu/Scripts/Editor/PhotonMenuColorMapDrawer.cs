using UnityEditor;
using UnityEngine;

namespace Photon.Menu
{
  [CustomPropertyDrawer(typeof(PhotonMenuColorMap))]
  public class PhotonMenuColorMapDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      EditorGUI.BeginProperty(position, label, property);
      position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
      var indent = EditorGUI.indentLevel;
      EditorGUI.indentLevel = 0;
      var space = 10;
      var width = position.width / 3 - space;
      var htmlColorRect = new Rect(position.x, position.y, width, position.height);
      var sourceColorRect = new Rect(position.x + width, position.y, width, position.height);
      var targetColorRect = new Rect(position.x + width * 2 + space, position.y, width, position.height);

      using (new EditorGUI.DisabledScope(true)) { 
        EditorGUI.PropertyField(htmlColorRect, property.FindPropertyRelative("Source"), GUIContent.none);
        var prop = property.FindPropertyRelative("Source");
        if (ColorUtility.TryParseHtmlString($"#{prop.stringValue}", out var color)) {
          EditorGUI.ColorField(sourceColorRect, color);
        }
      }
      EditorGUI.PropertyField(targetColorRect, property.FindPropertyRelative("Target"), GUIContent.none);
      EditorGUI.indentLevel = indent;
      EditorGUI.EndProperty();
    }
  }
}