using System;
using System.Threading.Tasks;
using Fusion.Photon.Realtime;
using Photon.Menu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Fusion.Menu {
  public class FusionMenuConnection : IPhotonMenuConnection {
    
    private NetworkRunner _runner;

    public UnityEvent<NetworkRunner> OnGameStartCallbacks;

    public NetworkRunner NetworkRunnerPrefab;

    public IPhotonMenuUIController UIController { get; set; }
    
    public bool IsSessionOwner => _runner.IsSceneAuthority;

    public string SessionName => _runner.SessionInfo.Name;

    public int MaxPlayerCount => _runner.SessionInfo.MaxPlayers;

    public virtual async Task ConnectAsync(IPhotonMenuConnectArgs connectionArgs)
    {
      UIController.Show<PhotonMenuLoadingUI>();
      _runner = CreateNewRunner();

      var fusionConnectionArgs = (FusionConnectArgs)connectionArgs;
      var startArgs = SetupStartArgs(fusionConnectionArgs);
      startArgs.OnGameStarted = runner => OnGameStartCallbacks.Invoke(runner);

      // Handle quick join session creation.
      if (connectionArgs.Creating == false && string.IsNullOrEmpty(connectionArgs.Session))
      {
        startArgs.EnableClientSessionCreation = false;
        var randomJoinResult = await StartRunner(startArgs);

        if (randomJoinResult.Success)
        {
          await StartGame(fusionConnectionArgs.UnityScene);
          return;
        }
        
        if (randomJoinResult.FailReason == ConnectFailReason.UserRequest)
        {
          return;
        }

        _runner = CreateNewRunner(); // create new runner if failed to find a random session to join.
        startArgs.EnableClientSessionCreation = true;
        startArgs.SessionName = UniqueSessionCodeGenerator.GetRandomCode(6);
        startArgs.GameMode = fusionConnectionArgs.GameMode == FusionGameMode.AuthoritativeServer ? GameMode.Host : GameMode.Shared;
      }

      var result = await StartRunner(startArgs);

      if (result.Success)
      {
        await StartGame(fusionConnectionArgs.UnityScene);
        return;
      }
      
      await DisconnectAsync(result.FailReason);
    }
    public virtual async Task DisconnectAsync(ConnectFailReason reason)
    {
      if (_runner)
      {
        await _runner.Shutdown();
      }
      
      var sceneCount = SceneManager.sceneCount;
      for (var i = sceneCount - 1; i > 0; i--) {
        // check if the scene is valid
        var scene = SceneManager.GetSceneAt(i);
        if (scene.IsValid() == false) continue;
        
        // start the unload
        var op = SceneManager.UnloadSceneAsync(scene);
        if (op == null) { continue; }
          
        // wait the unload to finish
        while (op.isDone == false) { await Task.Yield(); }
      }

      if (reason != ConnectFailReason.UserRequest)
      {
        await UIController.PopupAsync(reason.ToString(), "Disconnected");
      }

      UIController.Show<PhotonMenuMainUI>();
    }

    private NetworkRunner CreateNewRunner() {
      var runner = default(NetworkRunner);
      
      if (NetworkRunnerPrefab) {
        runner = Object.Instantiate(NetworkRunnerPrefab);
        runner.ProvideInput = true;
      } else {
        var obj = new GameObject("NetworkRunner");
        runner = obj.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
      }

      return runner;
    }

    private async Task StartGame(string sceneName)
    {
      try
      {
        if (_runner.IsSceneAuthority) {
          await _runner.LoadScene(sceneName, LoadSceneMode.Additive);
        }
        UIController.Show<PhotonMenuGameplayUI>();
      } catch (ArgumentException e)
      {
        Debug.LogError($"Failed to load scene. {e}.");
        await DisconnectAsync(ConnectFailReason.Disconnected);
      }
    }
        
    private async Task<ConnectResult> StartRunner(StartGameArgs args)
    {
      var result = await _runner.StartGame(args);

      var connectResult = new ConnectResult() {
        Success = _runner.IsRunning, 
        FailReason = ConvertFailReason(result.ShutdownReason)
      };

      return connectResult;
    }
    
    private ConnectFailReason ConvertFailReason(ShutdownReason reason)
    {
      switch (reason)
      {
        case ShutdownReason.Ok:
          return ConnectFailReason.UserRequest;
        case ShutdownReason.GameNotFound:
          return ConnectFailReason.GameNotFound;
        case ShutdownReason.GameIsFull:
          return ConnectFailReason.GameFull;
        case ShutdownReason.InvalidAuthentication:
          return ConnectFailReason.InvalidAuthentication;
        default:
          return ConnectFailReason.Disconnected;
      }
    }


    private StartGameArgs SetupStartArgs(FusionConnectArgs args)
    {
      var appSettings = PhotonAppSettings.Global.AppSettings.GetCopy();
      appSettings.AppVersion = args.AppVersion;
      appSettings.FixedRegion = args.Region;

      GameMode gameMode;
      switch (args.GameMode)
      {
        case FusionGameMode.Shared:
          gameMode = GameMode.Shared;
          break;
        case FusionGameMode.AuthoritativeServer:
          gameMode = args.Creating ? GameMode.Host : GameMode.Client;
          break;
        default:
          gameMode = GameMode.Host;
          break;
      }

      var startArgs = new StartGameArgs() { 
        SessionName = args.Session,
        PlayerCount = args.MaxPlayerCount,
        GameMode = gameMode,
        CustomPhotonAppSettings = appSettings as FusionAppSettings 
      };

      return startArgs;
    }
  }
}
