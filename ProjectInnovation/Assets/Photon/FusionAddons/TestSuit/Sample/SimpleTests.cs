using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Fusion.Addons.TestSuit {
  using Assert = NUnit.Framework.Assert;

  public class SimpleTests : TestBase {
    
    /// <summary>
    /// Simple Test that will start a Server and connect the defined number of clients
    /// </summary>
    [UnityTest]
    public IEnumerator ConnectTest([ValueSource(nameof(DefaultTestCases))] TestSetup testSetup) => TestUtils.TestTask(testSetup, async (setup) => {
      // Start Main Peer
      var serverStartGameArgs = TestUtils.CreateDefaultStartGameArgs(setup.ServerMode, setup.SessionName);

      var (startGameTask, runner) = TestUtils.CreateDefaultNetworkRunner(serverStartGameArgs);

      var result = await startGameTask;

      Assert.IsTrue(result.Ok);
      Assert.IsTrue(runner);
      Assert.IsTrue(runner.IsRunning);
      Assert.IsTrue(runner.IsCloudReady || runner.GameMode == GameMode.Single);
      Assert.IsFalse(runner.IsShutdown);

      if (setup.ClientCount > 0) {
        var clientGameMode = GameMode.Client;

        switch (setup.ServerMode) {
          case GameMode.Shared:
            clientGameMode = GameMode.Shared;
            break;
          case GameMode.Host:
          case GameMode.Server:
            clientGameMode = GameMode.Client;
            break;
          default:
            Assert.Fail();
            break;
        }

        var clientTasks = new List<Task<StartGameResult>>();
        var clientRunners = new List<NetworkRunner>();

        for (var i = 0; i < setup.ClientCount; i++) {
          var clientStartGameArgs = TestUtils.CreateDefaultStartGameArgs(clientGameMode, setup.SessionName);
          var (clientGameTask, runnerClient) = TestUtils.CreateDefaultNetworkRunner(clientStartGameArgs);

          clientTasks.Add(clientGameTask);
          clientRunners.Add(runnerClient);
        }

        await Task.WhenAll(clientTasks);

        clientTasks.ForEach((startGameResultTask) => {
          var startGameResult = startGameResultTask.Result;

          Assert.IsTrue(startGameResult.Ok);
        });

        clientRunners.ForEach((runnerClient) => {
          Assert.IsTrue(runnerClient);
          Assert.IsTrue(runnerClient.IsRunning);
          Assert.IsTrue(runnerClient.IsCloudReady);
          Assert.IsFalse(runnerClient.IsShutdown);

          if (setup.ServerMode == GameMode.Shared) {
            Assert.AreEqual(ConnectionType.Relayed, runnerClient.CurrentConnectionType);
          } else {
            Assert.AreEqual(ConnectionType.Direct, runnerClient.CurrentConnectionType);
          }
        });
      }
    });
  }
}