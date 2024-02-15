using Photon.Menu;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Fusion.Menu {
  public class FusionSettingsUI : PhotonMenuSettingsUI {
    [SerializeField] private TMP_Dropdown _gameModeField;

    public new FusionMenuConfig Config => (FusionMenuConfig)base.Config;
    public new FusionConnectArgs ConnectionArgs => (FusionConnectArgs)base.ConnectionArgs;

    protected override void Awake() {
      base.Awake();

      _gameModeField.onValueChanged.AddListener(_ => SaveChanges());
    }

    public override void Hide() {
      ConnectionArgs.GameMode = Config.AvailableGameModes[_gameModeField.value];

      base.Hide();
    }

    public override void SaveChanges() {
      base.SaveChanges();

      ConnectionArgs.GameMode = Config.AvailableGameModes[_gameModeField.value];
#if UNITY_IOS || UNITY_ANDROID
      if (ConnectionArgs.GameMode == FusionGameMode.AuthoritativeServer) {
        var sharedIndex = _gameModeField.options.FindIndex(data => data.text.Contains("Shared"));
        _gameModeField.SetValueWithoutNotify(sharedIndex);
        Controller.Popup("Authoritative server is NOT recommended on mobile. Using shared instead.", "Incompatible Topology");
        ConnectionArgs.GameMode = FusionGameMode.Shared;
      }
#endif
    }

    protected override void SetupFields() {
      base.SetupFields();

      var availableGameModes = Config.AvailableGameModes;

      _gameModeField.ClearOptions();
      _gameModeField.AddOptions(availableGameModes.Select(g => Enum.GetName(typeof(FusionGameMode), g)).ToList());

      var gameModeIndex = Config.AvailableGameModes.FindIndex(s => s == ConnectionArgs.GameMode);
      if (gameModeIndex >= 0) {
        _gameModeField.SetValueWithoutNotify(gameModeIndex);
      } else {
        _gameModeField.SetValueWithoutNotify(0);
        ConnectionArgs.GameMode = Config.AvailableGameModes[0];
      }
    }

    public override void LogToolTip(string id) {
      base.LogToolTip(id);
      switch (id) {
        case "FusionMode":
          Controller.Popup("Select GameMode to start the Fusion game in.\n\n" +
                           "AuthoritativeServer\n" +
                           "Shared", id);
          break;
      }
    }
  }
}