using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion.Addons.TestSuit {
  using Object = UnityEngine.Object;
  using Assert = NUnit.Framework.Assert;
  using TestContext = NUnit.Framework.TestContext;
  using InconclusiveException = NUnit.Framework.InconclusiveException;

  /// <summary>
  /// Set of Utility methods for Fusion Test
  /// </summary>
  public static class TestUtils {
    /// <summary>
    /// Default Fusion AppID used by the test peers
    /// </summary>
    private const string DefaultAppID = "";
    
    /// <summary>
    /// Default Application Version used by the test peers
    /// </summary>
    private const string DefaultAppVersion = "fusion-unit-test-runner";

    /// <summary>
    /// Creates a new <see cref="NetworkProjectConfig"/> with all the default values
    /// </summary>
    public static NetworkProjectConfig CreateDefaultProjectConfig() => new NetworkProjectConfig();

    /// <summary>
    /// Create a new <see cref="FusionAppSettings"/> with all default values
    /// </summary>
    /// <returns></returns>
    public static FusionAppSettings CreateDefaultAppSettings() {
      var appSettings = new FusionAppSettings() { AppIdFusion = DefaultAppID, AppVersion = DefaultAppVersion };

      // removes any nulls
      appSettings = JsonUtility.FromJson<FusionAppSettings>(JsonUtility.ToJson(appSettings));

      return appSettings;
    }

    /// <summary>
    /// Create a new <see cref="StartGameArgs"/>
    /// </summary>
    /// <param name="gameMode"><see cref="GameMode"/> used by the StartGameArgs</param>
    /// <param name="sessionName">Session Name</param>
    /// <param name="port">[optional] Peer binding port. Defaults to 0.</param>
    /// <param name="sceneManager">[optional] Custom <see cref="INetworkSceneManager"/>. Defaults to <see cref="TestSceneManager"/>.</param>
    /// <param name="appSettings">[optional] Custom <see cref="FusionAppSettings"/>. Defaults to <see cref="CreateDefaultAppSettings"/>.</param>
    /// <param name="networkConfig">[optional] Custom <see cref="NetworkProjectConfig"/>. Defaults to <see cref="CreateDefaultProjectConfig"/>.</param>
    /// <param name="sceneInfo">[optional] Custom <see cref="NetworkSceneInfo"/>. Defaults to the current loaded scene.</param>
    /// <returns></returns>
    public static StartGameArgs CreateDefaultStartGameArgs(
      GameMode gameMode,
      string sessionName,
      ushort port = 0,
      INetworkSceneManager sceneManager = null,
      FusionAppSettings appSettings = null,
      NetworkProjectConfig networkConfig = null,
      NetworkSceneInfo? sceneInfo = null) {

      if (sceneInfo == null && TryGetCurrentSceneRef(out var sceneRef)) {
        sceneInfo = new NetworkSceneInfo();
        sceneInfo.Value.AddSceneRef(sceneRef);
      }
      
      return new StartGameArgs {
        CustomPhotonAppSettings = appSettings ?? CreateDefaultAppSettings(),
        Config = networkConfig ?? CreateDefaultProjectConfig(),
        GameMode = gameMode,
        Address = NetAddress.Any(port),
        Scene = sceneInfo,
        SessionName = sessionName,
        SceneManager = sceneManager ?? new TestSceneManager(),
      };
    }
    
    /// <summary>
    /// Try to get a <see cref="SceneRef"/> of the currently loaded scene.
    /// </summary>
    /// <param name="sceneRef">Current loaded SceneRef or default if invalid.</param>
    /// <returns>True if a valid scene is loaded, false otherwise.</returns>
    public static bool TryGetCurrentSceneRef(out SceneRef sceneRef) {
      var activeScene = SceneManager.GetActiveScene();
      if (activeScene.buildIndex < 0 || activeScene.buildIndex >= SceneManager.sceneCountInBuildSettings) {
        sceneRef = default;
        return false;
      }

      sceneRef = SceneRef.FromIndex(activeScene.buildIndex);
      return true;
    }

    /// <summary>
    /// Setup a new <see cref="NetworkRunner"/>
    /// </summary>
    /// <param name="startGameArgs">NetworkRunner StartGameArgs</param>
    /// <param name="runnerPrefab">[optional] <see cref="NetworkRunner"/> Base prefab</param>
    /// <param name="networkRunnerCallbacks">[optional] <see cref="INetworkRunnerCallbacks"/> list</param>
    /// <returns></returns>
    public static (Task<StartGameResult>, NetworkRunner) CreateDefaultNetworkRunner(
      StartGameArgs startGameArgs,
      NetworkRunner runnerPrefab = null,
      INetworkRunnerCallbacks[] networkRunnerCallbacks = null) {
      Assert.IsFalse(startGameArgs.Equals(default));

      runnerPrefab = runnerPrefab ? runnerPrefab : new GameObject("RunnerPrefab").AddComponent<NetworkRunner>();

      var networkRunnerInstance = Object.Instantiate(runnerPrefab);
      networkRunnerInstance.name = $"{startGameArgs.GameMode}-{Guid.NewGuid()}";
      Object.DontDestroyOnLoad(networkRunnerInstance);

      if (networkRunnerCallbacks != null) {
        networkRunnerInstance.AddCallbacks(networkRunnerCallbacks);
      }

      return (networkRunnerInstance.StartGame(startGameArgs), networkRunnerInstance);
    }

    /// <summary>
    /// Run a <see cref="TestSetup"/> as <see cref="Task"/> in order to be executed by the TestSuit
    /// </summary>
    /// <param name="setup"><see cref="TestSetup"/> used by the test</param>
    /// <param name="testAction">Test implementation Task that will be wrapped</param>
    /// <exception cref="TimeoutException">If the test executed exceeds the <see cref="TestContext.CurrentTestExecutionContext.TestCaseTimeout"/>.</exception>
    /// <exception cref="Exception">If any exceptions happen on the test itself.</exception>
    /// <exception cref="AggregateException">If a internal Task exception occurs.</exception>
    public static IEnumerator TestTask(TestSetup setup, Func<TestSetup, Task> testAction) {
      var timeout = TestContext.CurrentTestExecutionContext.TestCaseTimeout;

      Log.Init(new TestLogger() { LogLevel = LogType.Warn });

      yield return SceneManager.LoadSceneAsync("TestEmptyScene");

      var sw = Stopwatch.StartNew();
      try {
        using (var task = testAction(setup)) {
          while (task.IsCompleted == false) {
            if (sw.ElapsedMilliseconds > timeout) {
              yield return ShutdownAll();
              throw new TimeoutException();
            }

            yield return null;
          }

          if (task.IsFaulted) {
            var ex = task.Exception;
            if (ex != null && ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is InconclusiveException) {
              throw ex.InnerExceptions[0];
            }

            if (task.Exception != null) throw task.Exception;
          }
        }

        // shutdown all the runners
        yield return ShutdownAll();
      } finally {
        Log.Init(null);
      }
    }

    /// <summary>
    /// Shutdown all active <see cref="NetworkRunner"/>
    /// </summary>
    /// <exception cref="AggregateException">If any of the Shutdown tasks is faulted.</exception>
    public static IEnumerator ShutdownAll() {
      var tasks = new List<Task>();
      var runners = NetworkRunner.Instances.ToList();
      foreach (var runner in runners) {
        if (runner != null && runner.IsRunning) {
          tasks.Add(runner.Shutdown());
        }
      }

      foreach (var task in tasks) {
        while (task.IsCompleted == false) {
          yield return null;
        }

        if (task.IsFaulted && task.Exception != null) {
          throw task.Exception;
        }
      }

      yield return null;
    }
  }
}