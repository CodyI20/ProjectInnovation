using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Photon.Menu {
  public class PhotonMenuSettingsUI : PhotonMenuUIScreen {
    [SerializeField] protected TMP_Dropdown _regionField;
    [SerializeField] protected TMP_InputField _maxPlayersField;
    [SerializeField] protected TMP_Dropdown _appVersionField;

    protected Regex EscapeExpression = new Regex(@"\[(?<name>.*)\]", RegexOptions.Compiled);
    private List<string> _regionMap = new List<string>();
    private List<string> _appVersionMap = new List<string>();

    protected new virtual void Awake()
    {
      _regionField.onValueChanged.AddListener(_ => SaveChanges());
      _maxPlayersField.onValueChanged.AddListener(_ => SaveChanges());
      _appVersionField.onValueChanged.AddListener(_ => SaveChanges());
    }

    public override void Show() {
      base.Show();
      SetupFields();
    }

    public virtual void LogToolTip(string id) {
      switch (id) {
        case "AppVersion":
          Controller.Popup("Select the AppVersion to separate player with different build in the Photon matchmaking.", id);
          break;
        case "MaxPlayers":
          Controller.Popup("Select the max player count for all games.", id);
          break;
        case "Region":
          Controller.Popup("Select the Region to connect to. Use Best to connect to the closest Photon cloud available.", id);
          break;
      }
    }

    public virtual void SaveChanges() 
    {
      ConnectionArgs.Region = _regionMap[_regionField.value];

      ConnectionArgs.AppVersion = _appVersionMap[_appVersionField.value];

      if (Int32.TryParse(_maxPlayersField.text, out var maxPlayers) == false)
      {
        maxPlayers = Config.MaxPlayerCount;
      }

      maxPlayers = Mathf.Clamp(maxPlayers, 1, Config.MaxPlayerCount);

      _maxPlayersField.SetTextWithoutNotify(maxPlayers.ToString());
      ConnectionArgs.MaxPlayerCount = maxPlayers;
    }

    protected virtual void SetupFields()
    {
      // Create mapping, machine guid uses the escaped [name] from the available regions
      _regionMap.Clear();
      _regionField.ClearOptions();
      for (int i = 0; i < Config.AvailableRegions.Count; i++) {
        var s = Config.AvailableRegions[i];
        _regionMap.Add(TryToDeEscape(ref s) ? string.Empty : s);
        _regionField.options.Add(new TMP_Dropdown.OptionData(s));
      }
      _regionField.RefreshShownValue();

      // Find initial position, add an extra one if not found in default lists
      var regionIndex = _regionMap.FindIndex(s => s != null && s.Equals(ConnectionArgs.Region, StringComparison.InvariantCulture));
      if (regionIndex >= 0) {
        _regionField.SetValueWithoutNotify(regionIndex);
      } else {
        _regionMap.Add(ConnectionArgs.Region);
        _regionField.options.Add(new TMP_Dropdown.OptionData(ConnectionArgs.Region));
        _regionField.SetValueWithoutNotify(_regionMap.Count - 1);
      }

      // Create mapping, machine guid uses the escaped [name] from the available regions
      _appVersionMap.Clear();
      _appVersionField.ClearOptions();
      for (int i = 0; i < Config.AvailableAppVersions.Count; i++) {
        var s = Config.AvailableAppVersions[i];
        _appVersionMap.Add(TryToDeEscape(ref s) ? Config.MachineGuid : s);
        _appVersionField.options.Add(new TMP_Dropdown.OptionData(s));
      }
      _appVersionField.RefreshShownValue();

      // Find initial position, add an extra one if not found in default lists
      var appVersionIndex = _appVersionMap.FindIndex(s => s.Equals(ConnectionArgs.AppVersion, StringComparison.InvariantCulture));
      if (appVersionIndex >= 0) {
        _appVersionField.SetValueWithoutNotify(appVersionIndex);
      }
      else {
        //_appVersionMap.Add(ConnectionArgs.Region);
        //_appVersionField.options.Add(new TMP_Dropdown.OptionData(ConnectionArgs.Region));
        _appVersionField.SetValueWithoutNotify(0);
      }

      // Setup max player input field, reset input value if needed
      var maxPlayerValue = ConnectionArgs.MaxPlayerCount;
      if (maxPlayerValue >= 0 || maxPlayerValue < Config.MaxPlayerCount)
      {
        _maxPlayersField.SetTextWithoutNotify(maxPlayerValue.ToString());
      }
      else
      {
        _maxPlayersField.SetTextWithoutNotify(Config.MaxPlayerCount.ToString());
        ConnectionArgs.MaxPlayerCount = Config.MaxPlayerCount;
      }
    }

    protected bool TryToDeEscape(ref string s) {
      try {
        var matches = EscapeExpression.Matches(s);
        if (matches != null && matches.Count > 0) {
          s = matches[0].Groups["name"].Value;
          return true;
        }
      } catch { }

      return false;
    }
  }
}
