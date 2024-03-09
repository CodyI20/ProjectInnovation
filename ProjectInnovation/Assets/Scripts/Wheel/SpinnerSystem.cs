using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CookingEnums;
using Fusion;
using System.Threading;
using System.Threading.Tasks;

public class WheelSystem : MonoBehaviour
{
    [System.Serializable]
    public class WheelItem
    {
        public RawIngredients ingredient;
        public int size;
    }

    private bool spinning = false;
    private Inventory inventory;

    // Used to rotate around a point if null; it will rotate around itself
    public Transform rotatePoint;
    public float suspense = 5.0f; // Time in seconds for which the wheel spins
    public List<WheelItem> caseItems = new List<WheelItem>();
    private float initialForce;
    private CancellationTokenSource spinCancellationTokenSource;

    private void OnEnable()
    {
        Debug.Log("WheelSystem enabled");
        GameManager.Instance.OnPlayerTurnEnd += TurnOffWheel;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPlayerTurnEnd -= TurnOffWheel;
    }


    public void GetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }

    private void Update()
    {
        if (!spinning && (Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            StartSpinningAsync();
        }
    }

    private async void StartSpinningAsync()
    {
        if (!spinning)
        {
            spinning = true;
            spinCancellationTokenSource = new CancellationTokenSource();

            try
            {
                await SpinWheelAsync(spinCancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                // Task was canceled (game object disabled), ignore or handle as needed
            }
        }
    }

    private void StopSpinningAsync()
    {
        if (spinCancellationTokenSource != null)
        {
            spinCancellationTokenSource.Cancel();
            spinning = false;
        }
    }

    private async Task SpinWheelAsync(CancellationToken cancellationToken)
    {
        initialForce = Random.Range(1500, 3000);
        float startTime = Time.time;
        float endTime = startTime + suspense;

        while (Time.time < endTime)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            float elapsed = Time.time - startTime;
            float t = elapsed / suspense;
            float easedT = EaseOutQuad(t);

            float rotationSpeed = Mathf.Lerp(initialForce, 0f, easedT);

            if (rotatePoint != null)
                transform.RotateAround(rotatePoint.position, Vector3.forward, rotationSpeed * Time.deltaTime);
            else
                transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            await Task.Yield();
        }

        FinishSpinning();
    }

    private void FinishSpinning()
    {
        spinning = false;

        float rotationAngle = transform.eulerAngles.z;
        float totalPartSize = 0;

        foreach (WheelItem item in caseItems)
        {
            totalPartSize += item.size;
        }

        float partSize = 360 / totalPartSize;
        Debug.Log("Rotation angle: " + rotationAngle + " part size: " + partSize + " rotation angle / part size: " + rotationAngle / partSize);

        int part = (int)(rotationAngle / partSize);
        bool success = false;

        foreach (WheelItem item in caseItems)
        {
            part -= item.size;
            if (part <= 0)
            {
                Debug.Log("Landed on: " + item.ingredient);
                GiveItem(item.ingredient);
                TurnOffWheel(GameManager.Instance.Runner.LocalPlayer);
                success = true;
                break;
            }
        }

        if (!success)
        {
            Debug.Log("Failed to select");
        }
    }

    private void TurnOffWheel(PlayerRef player)
    {
        Debug.Log("Turning off wheel");
        StopSpinningAsync();
        gameObject.SetActive(false);
    }

    private void GiveItem(RawIngredients ingredient)
    {
        if (inventory != null)
        {
            inventory.AddIngredient(ingredient);
        }
    }

    private void GiveItem(RawIngredients ingredient, int quantity)
    {
        if (inventory != null)
            inventory.AddIngredient(ingredient, quantity);
    }

    private float EaseOutQuad(float t)
    {
        return 1 - (1 - t) * (1 - t);
    }
}
