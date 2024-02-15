using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Fusion.Addons.TestSuit {
  /// <summary>
  /// Base class used by the Unit Tests
  /// </summary>
  public abstract class TestBase {
    /// <summary>
    /// Shutdown all NetworkRunners
    /// </summary>
    [UnityTearDown]
    public IEnumerator TearDown() {
      Debug.Log("Test TearDown. Shutdown all Runners");
      return TestUtils.ShutdownAll();
    }

    /// <summary>
    /// Tests with only 1 running peer
    /// </summary>
    protected static IEnumerable<TestSetup> TestCasesSingle => DefaultTestCases.Where(x => x.TotalPeers == 1);

    /// <summary>
    /// Tests with more than 1 running peer
    /// </summary>
    protected static IEnumerable<TestSetup> TestCasesWithRemotes => DefaultTestCases.Where(x => x.TotalPeers > 1);

    /// <summary>
    /// Tests only with Shared Mode Clients
    /// </summary>
    protected static IEnumerable<TestSetup> TestCasesWithRemotesShared => TestCasesWithRemotes.Where(x => x.ServerMode == GameMode.Shared);

    /// <summary>
    /// Tests only with ClientServer peers
    /// </summary>
    protected static IEnumerable<TestSetup> TestCasesWithRemotesClientServer => TestCasesWithRemotes.Where(x => x.ServerMode != GameMode.Shared);

    /// <summary>
    /// Tests only with ClientServer peers at max 1 client
    /// </summary>
    protected static IEnumerable<TestSetup> TestCasesWithSingleRemotesClientServer => TestCasesWithRemotes.Where(x => x.ServerMode != GameMode.Shared && x.ClientCount == 1);

    /// <summary>
    /// Build all default test cases
    /// It will cycle between all Replication Modes and Game Modes
    /// </summary>
    protected static IEnumerable<TestSetup> DefaultTestCases {
      get {
        foreach (var clientCount in new[] { 0, 1, 4 }) {
          foreach (var server in new[] { GameMode.Single, GameMode.Server, GameMode.Host, GameMode.Shared }) {
            if (server == GameMode.Single && clientCount > 0) {
              continue;
            }

            var actualClientCount = clientCount;
            if (server == GameMode.Shared) {
              actualClientCount = Math.Min(4, clientCount + 1);
            }

            yield return new TestSetup { ServerMode = server, ClientCount = actualClientCount, SessionName = Guid.NewGuid().ToString() };
          }
        }
      }
    }
  }
}