using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion.Addons.TestSuit {
  using Object = UnityEngine.Object;
  using Assert = NUnit.Framework.Assert;

  /// <summary>
  /// Default SceneManager used internally on Tests
  /// </summary>
  public class TestSceneManager : INetworkSceneManager {
    private NetworkRunner _runner;
    private Scene _scene;
    private bool _isReady;

    public void SetScene(Scene scene) {
      _scene = scene;
    }

    public void SetReady(bool isReady) {
      _isReady = isReady;
    }

    public async Task<NetworkObject[]> LoadSceneAsyncAndRegisterObjects(string sceneName, SceneRef sceneRef = default) {
      Scene scene = default;
      Scene prevScene = default;

      Assert.AreSame(this, _runner.SceneManager);

      _runner.InvokeSceneLoadStart(sceneRef);

      if (_runner.Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple) {
        prevScene = _runner.SimulationUnityScene;

        var physicsMode = LocalPhysicsMode.Physics2D | LocalPhysicsMode.Physics3D;
        var op = SceneManager.LoadSceneAsync(sceneName, new LoadSceneParameters(LoadSceneMode.Additive, physicsMode));
        Assert.NotNull(op);

        scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        Assert.IsTrue(scene.IsValid());

        await Task.Run(() => {
          while (op.isDone == false) {
            Task.Delay(10);
          }
        });

        Assert.IsTrue(scene.isLoaded);
      } else {
        var op = SceneManager.LoadSceneAsync(sceneName);

        await Task.Run(() => {
          while (op.isDone == false) {
            Task.Delay(10);
          }
        });

        scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
      }

      SetScene(scene);

      if (prevScene.IsValid()) {
        var op = SceneManager.UnloadSceneAsync(prevScene);

        await Task.Run(() => {
          while (op.isDone == false) {
            Task.Delay(10);
          }
        });
      }

      Assert.AreEqual(sceneName, scene.name);

      var sceneObjects = scene.GetRootGameObjects()
        .SelectMany(x => x.GetComponentsInChildren<NetworkObject>(includeInactive: true))
        .ToArray();

      // deactivate before attaching
      foreach (var obj in sceneObjects) {
        obj.gameObject.SetActive(false);
      }

      _runner.RegisterSceneObjects(sceneRef, sceneObjects);
      _runner.InvokeSceneLoadDone(new SceneLoadDoneArgs(sceneRef, sceneObjects));

      SetReady(true);
      return sceneObjects;
    }

    public void Initialize(NetworkRunner runner) {
      Assert.IsNull(_runner, $"Already is {_runner}");
      _runner = runner;
    }

    public void Shutdown() {
      Assert.NotNull(_runner);
      _runner = null;
    }

    public bool IsBusy => !_isReady;
    public Scene MainRunnerScene => _scene;

    public bool TryGetPhysicsScene2D(out PhysicsScene2D scene2D) {
      if (_scene.IsValid()) {
        scene2D = _scene.GetPhysicsScene2D();
        return true;
      }

      scene2D = default;
      return false;
    }

    public bool TryGetPhysicsScene3D(out PhysicsScene scene3D) {
      if (_scene.IsValid()) {
        scene3D = _scene.GetPhysicsScene();
        return true;
      }

      scene3D = default;
      return false;
    }

    public void MakeDontDestroyOnLoad(GameObject obj) {
      Object.DontDestroyOnLoad(obj);
    }

    public void MoveToRunnerScene(GameObject obj) {
    }

    public bool IsRunnerScene(Scene scene) {
      return _scene == scene;
    }

    public bool MoveGameObjectToScene(GameObject gameObject, SceneRef sceneRef) {
      return true;
    }

    public SceneRef GetSceneRef(GameObject gameObject) {
      throw new NotImplementedException();
    }

    public NetworkSceneAsyncOp LoadScene(SceneRef sceneRef, NetworkLoadSceneParameters parameters) {
      throw new NotImplementedException();
    }

    public NetworkSceneAsyncOp UnloadScene(SceneRef sceneRef) {
      throw new NotImplementedException();
    }

    public void OnSceneInfoChanged() {
    }

    public SceneRef GetSceneRef(string sceneNameOrPath) {
      return SceneRef.FromIndex(FusionUnitySceneManagerUtils.GetSceneBuildIndex(sceneNameOrPath));
    }

    public bool OnSceneInfoChanged(NetworkSceneInfo sceneInfo, NetworkSceneInfoChangeSource changeSource) {
      return false;
    }
  }
}