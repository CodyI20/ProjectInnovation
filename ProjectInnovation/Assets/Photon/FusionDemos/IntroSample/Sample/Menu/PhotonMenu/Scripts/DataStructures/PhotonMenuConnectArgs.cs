using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Menu {
  public class PhotonMenuConnectArgs : IPhotonMenuConnectArgs {
    private string _session;
    private bool _creating;
    
    public virtual string Username
    {
      get => PlayerPrefs.GetString("Photon.Menu.Username");
      set => PlayerPrefs.SetString("Photon.Menu.Username", value);
    }

    public virtual string Session
    {
      get => _session;
      set => _session = value;
    }

    public virtual string Region
    {
      get => PlayerPrefs.GetString("Photon.Menu.Region", String.Empty);
      set => PlayerPrefs.SetString("Photon.Menu.Region", value);
    }

    public virtual string AppVersion
    {
      get => PlayerPrefs.GetString("Photon.Menu.AppVersion", String.Empty);
      set => PlayerPrefs.SetString("Photon.Menu.AppVersion", value);
    }

    public virtual int MaxPlayerCount
    {
      get => PlayerPrefs.GetInt("Photon.Menu.MaxPlayerCount", 4);
      set => PlayerPrefs.SetInt("Photon.Menu.MaxPlayerCount", value);
    }

    public virtual bool Creating
    {
      get => _creating;
      set => _creating = value;
    }

    public virtual string InformationalVersion {
      get {
        var list = new List<string>();
        if (string.IsNullOrEmpty(Region) == false) {
          list.Add(Region);
        }
        if (string.IsNullOrEmpty(AppVersion) == false) {
          list.Add(AppVersion);
        }
        if (list.Count == 0) {
          return null;
        }
        return string.Join(" | ", list);
      }
    }

    public virtual void SetDefaults(IPhotonMenuConfig config)
    {
      Session = null;
      Creating = false;
      
      if (AppVersion == String.Empty) {
        var appVersion = String.Empty;
        if (config.AvailableAppVersions.Count > 0) {
          appVersion = config.AvailableAppVersions[0];
        } else {
          Debug.LogWarning("No app version selected.");
        }
        AppVersion = appVersion;
      }

      if (Region != String.Empty && config.AvailableRegions.Contains(Region) == false)
      {
        Region = string.Empty;
      }

      if (MaxPlayerCount <= 0 || MaxPlayerCount > config.MaxPlayerCount)
      {
        MaxPlayerCount = Mathf.Clamp(MaxPlayerCount, 1, config.MaxPlayerCount);
      }

      if (string.IsNullOrEmpty(Username))
      {
        Username = $"Player{UniqueSessionCodeGenerator.GetRandomCode(3)}";
      }
    }
  }
}
