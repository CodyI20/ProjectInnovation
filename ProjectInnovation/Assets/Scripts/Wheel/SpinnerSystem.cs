using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CookingEnums;
using Fusion;

public class WheelSystem : MonoBehaviour
{
    [System.Serializable]
    public class WheelItem
    {
        public RawIngredients ingredient;
        public int size;
    }

    private void OnEnable()
    {
        Debug.Log("WheelSystem enabled");
        GameManager.Instance.OnPlayerTurnEnd += TurnOffWheel;
    }


    // Used to rotate around a point if null, it will rotate around itself
    public Transform rotatePoint;
    public float suspense = 1.0f;
    public List<WheelItem> caseItems = new List<WheelItem>();
    private float randomForce;
    private bool spinning = false;
    private Inventory inventory;

    void Start()
    {
        
    }

    public void GetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            LaunchWheel(inventory);
        if (spinning) {
            if (rotatePoint != null)
                transform.RotateAround(rotatePoint.position, Vector3.forward, randomForce * Time.deltaTime);
            else
                transform.Rotate(Vector3.forward, randomForce * Time.deltaTime);
            randomForce = Mathf.Lerp(randomForce, 0, 0.1f/suspense);
            if (randomForce < 1.0f) {
                spinning = false;
                randomForce = 0;
                float rotationAngle = transform.eulerAngles.z;
                float totalPartSize = 0;
                foreach (WheelItem item in caseItems) {
                    totalPartSize += item.size;
                }
                float partSize = 360 / totalPartSize;
                Debug.Log("Rotation angle: " + rotationAngle + " part size: " + partSize + " rotation angle / part size: " + rotationAngle / partSize);
                int part = (int)(rotationAngle / partSize);
                bool success = false;
                foreach (WheelItem item in caseItems) {
                    part -= item.size;
                    if (part <= 0) {
                        Debug.Log("Landed on: " + item.ingredient);
                        GiveItem(item.ingredient);
                        TurnOffWheel(GameManager.Instance.Runner.LocalPlayer);
                        success = true;
                        break;
                    }
                }
                if ( (!success)) 
                {
                    Debug.Log("Failed to select");
                }
            }
        }
    }

    void LaunchWheel(Inventory inventory) 
    {
        if (spinning)
            return;
        if (inventory != null)
            this.inventory = inventory;
        randomForce = Random.Range(1500, 3000);
        spinning = true;
    }

    private void TurnOffWheel(PlayerRef player)
    {
        spinning = false;
        gameObject.SetActive(false);
    }

    void GiveItem(RawIngredients ingredient)
    {
        if (inventory != null)
        {
            inventory.AddIngredient(ingredient);
        }
    }

    void GiveItem(RawIngredients ingredient, int quantity)
    {
        if (inventory != null)
            inventory.AddIngredient(ingredient, quantity);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPlayerTurnEnd -= TurnOffWheel;
    }
}
