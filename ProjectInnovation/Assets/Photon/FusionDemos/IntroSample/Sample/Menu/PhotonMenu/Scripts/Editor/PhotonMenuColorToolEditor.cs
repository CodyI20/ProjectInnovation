using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Menu
{
  [CustomEditor(typeof(PhotonMenuColorTool))]
  public class PhotonMenuColorToolEditor : UnityEditor.Editor {
    public override void OnInspectorGUI() {
      var data = (PhotonMenuColorTool)target;

      base.DrawDefaultInspector();

      if (GUILayout.Button("Write To Prefabs")) {
        var mapping = CreateDictionary(data.Palette.Mapping);
        for (int i = 0; i < data.Paths.Length; i++) {
          WritePrefab(data.Paths[i], mapping);
        }
      }

      if (GUILayout.Button("Rebuild Palette")) {
        var set = new HashSet<string>();
        for (int i = 0; i < data.Prefabs.Count; i++) {
          var assetPath = AssetDatabase.GetAssetPath(data.Prefabs[i]);
          ReadPrefab(assetPath, ref set);
        }
        data.Paths = set.ToArray();
        Array.Sort(data.Paths);
        RefreshMap(data);
      }

      var obj = EditorGUILayout.ObjectField("Add Prefab", null, typeof(UnityEngine.Object), true);
      if (obj != null) {
        // Save prefab reference to quickly rebuild the mapping
        if (data.Prefabs.Contains(obj) == false) {
          data.Prefabs.Add(obj);
        }

        // Create new paths and update the mapping with colors found
        var assetPath = AssetDatabase.GetAssetPath(obj);
        if (assetPath != null) {
          var set = new HashSet<string>(data.Paths);
          ReadPrefab(assetPath, ref set);
          data.Paths = set.ToArray();
          Array.Sort(data.Paths);
          RefreshMap(data);
          EditorUtility.SetDirty(data);
        }
      }
    }

    public static Dictionary<string, Color> CreateDictionary(PhotonMenuColorMap[] array) {
      var dict = new Dictionary<string, Color>();
      foreach (var mapping in array) {
        dict.Add(mapping.Source, mapping.Target);
      }
      return dict;
    }

    public static void RefreshMap(PhotonMenuColorTool data) {
      var dict = CreateDictionary(data.Palette.Mapping);

      foreach (var path in data.Paths) {
        var tokens = path.Split(';');
        var htmlColor = tokens[tokens.Length - 1];
        if (ColorUtility.TryParseHtmlString($"#{htmlColor}", out var color)) {
          if (dict.ContainsKey(htmlColor) == false) {
            dict.Add(htmlColor, color);
          }
        }
      }

      data.Palette.Mapping = dict.Select(m => new PhotonMenuColorMap { Source = m.Key, Target = m.Value }).ToArray();

      Array.Sort(data.Palette.Mapping, (a, b) => a.Source.CompareTo(b.Source));

      EditorUtility.SetDirty(data.Palette);
    }

    public static void ReadPrefab(string assetPath, ref HashSet<string> paths) {
      var contentsRoot = PrefabUtility.LoadPrefabContents(assetPath);
      ReadPrefab(contentsRoot, assetPath + ";", ref paths);
      PrefabUtility.UnloadPrefabContents(contentsRoot);
    }

    public static void ReadPrefab(GameObject node, string path, ref HashSet<string> paths) {
      path += $"{node.name}";
    
      var imageComponent = node.GetComponent<Image>();
      var textComponet = node.GetComponent<TMP_Text>();
      var toggleComponent = node.GetComponent<Toggle>();

      if (imageComponent) {
        paths.Add($"{path};{imageComponent.GetType().Name};{ColorUtility.ToHtmlStringRGBA(imageComponent.color)}");
      }

      if (textComponet) {
        paths.Add($"{path};{textComponet.GetType().Name};{ColorUtility.ToHtmlStringRGBA(textComponet.color)}");
      }

      if (toggleComponent) {
        paths.Add($"{path};{toggleComponent.GetType().Name}/highlightedColor;{ColorUtility.ToHtmlStringRGBA(toggleComponent.colors.highlightedColor)}");
        paths.Add($"{path};{toggleComponent.GetType().Name}/disabledColor;{ColorUtility.ToHtmlStringRGBA(toggleComponent.colors.disabledColor)}");
        paths.Add($"{path};{toggleComponent.GetType().Name}/normalColor;{ColorUtility.ToHtmlStringRGBA(toggleComponent.colors.normalColor)}");
        paths.Add($"{path};{toggleComponent.GetType().Name}/pressedColor;{ColorUtility.ToHtmlStringRGBA(toggleComponent.colors.pressedColor)}");
        paths.Add($"{path};{toggleComponent.GetType().Name}/selectedColor;{ColorUtility.ToHtmlStringRGBA(toggleComponent.colors.selectedColor)}");
      }

      path += "/";

      for (int i = 0; i < node.transform.childCount; i++) {
        ReadPrefab(node.transform.GetChild(i).gameObject, path, ref paths);
      }
    }

    public static void WritePrefab(string path, Dictionary<string, Color> mapping) {
      // Assets/PhotonMenu/UIPrototype/Prefabs/Buttons/PrimaryMenuButton.prefab;ButtonLabel/Background/PrimaryMenuButton;Image;FFFFFF00
      var tokens = path.Split(';');
      if (tokens.Length != 4) {
        Debug.LogError($"Token length incorrect when splitting: '{path}'");
        return;
      }

      var prefab = PrefabUtility.LoadPrefabContents(tokens[0]);

      var hierarchy = tokens[1].Split('/');
      var go = prefab;
      for (int i = 1; i < hierarchy.Length; i++) {
        var transform = go.transform.Find(hierarchy[i]);
        if (transform) {
          go = transform.gameObject;
        } else {
          Debug.LogError($"GameObject hierarchy cannot be resolved: '{tokens[1]}' at '{hierarchy[i]}' of path {path}");
          return;
        }
      }

      if (mapping.TryGetValue(tokens[3], out var color)) {
      } else {
        Debug.LogError($"Cannot map color '{tokens[3]}'");
      }

      var componentType = tokens[2].Split('/');
      var component = go.GetComponent(componentType[0]);
      if (component == null) {
        Debug.LogError($"Cannot find component type '{componentType[0]}' on GameObject '{go.name}' of path {path}");
        return;
      }

      if (component.GetType() == typeof(Image)) {
        ((Image)component).color = color;
      } else if (component.GetType() == typeof(TMP_Text)) {
        ((TMP_Text)component).color = color;
      } else if (component.GetType() == typeof(Toggle)) {
        if (componentType.Length < 1) {
          Debug.LogError($"Member information of Toggle component is missing in '{path}'");
          return;
        }
        var colorBlock = ((Toggle)component).colors;
        switch (componentType[1]) {
          case "highlightedColor": colorBlock.highlightedColor = color; break;
          case "disabledColor": colorBlock.highlightedColor = color; break;
          case "normalColor": colorBlock.highlightedColor = color; break;
          case "pressedColor": colorBlock.highlightedColor = color; break;
          case "selectedColor": colorBlock.highlightedColor = color; break;

        }
        ((Toggle)component).colors = colorBlock;
      }

      PrefabUtility.SaveAsPrefabAsset(prefab, tokens[0]);
      PrefabUtility.UnloadPrefabContents(prefab);
    }
  }
}