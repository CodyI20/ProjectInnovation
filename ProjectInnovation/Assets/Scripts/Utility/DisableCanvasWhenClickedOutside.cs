using UnityEngine;

public class DisableCanvasWhenClickedOutside : MonoBehaviour
{
    // Disable the canvas if you click/tap outside of it
    private void HideIfClickedOutside()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 inputPosition;

            if (Input.GetMouseButtonDown(0))
            {
                inputPosition = Input.mousePosition;
            }
            else
            {
                inputPosition = Input.GetTouch(0).position;
            }

            if (gameObject.activeSelf &&
                !RectTransformUtility.RectangleContainsScreenPoint(
                    GetComponent<RectTransform>(),
                    inputPosition,
                    Camera.main))
            {
                gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        HideIfClickedOutside();
    }
}
