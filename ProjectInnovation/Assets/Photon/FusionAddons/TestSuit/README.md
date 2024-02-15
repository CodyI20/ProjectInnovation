# Fusion TestSuite Add-On

> This package comprises a collection of utility classes and methods primarily designed as boilerplate for Unit Tests of the Fusion SDK. 
> Given that the Fusion start/shutdown process heavily relies on the C# Async/Await Task API, a custom wrapper was essential for testing purposes.

## Main Classes

1. `TestSetup`: Used to define a single test, specifying the type of server and the number of connected clients.
2. `TestLogger`: Internally employed for all Fusion tests as the custom logger.
3. `TestBase`: Base class utilized by the unit tests.
4. `TestSceneManager`: Default SceneManager internally used in tests.
5. `TestUtils`: Set of utility methods for Fusion testing.

## How to Use

1. Set a Test Photon Application ID at `TestUtils.DefaultAppID`. This is the AppID used by the peers to connect to the Photon Cloud.
   1. This AppID must be of type:
      - `Photon SDK`: `Fusion`
      - `SDK Version`: `Fusion 2`.
   2. If necessary, create a new one at [https://dashboard.photonengine.com/](https://dashboard.photonengine.com/).
2. Add the `Scenes/TestEmptyScene` to the `Scenes in Build`. It is loaded when running the tests.
3. Create a new Test Unit class and extend the `TestBase`.
4. Develop a new test following the pattern:

```csharp
public class SampleTest : TestBase {
    [UnityTest]
    public IEnumerator ConnectTest([ValueSource(nameof(DefaultTestCases))] TestSetup testSetup) => TestUtils.TestTask(testSetup, async (setup) => {   
       // Utilize TestSetup to execute the test itself, with the ability to await and run tasks.
    });
}
```

5. It is possible to replace `DefaultTestCases` with any other examples found in the `TestBase` class, each with a different set of peers.