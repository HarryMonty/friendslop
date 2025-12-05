using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    void Awake()
    {
        if (playerCamera == null)
        {
            Debug.Log("Player camera not found: forcing main camera as player.");
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    // Handle click function
    private void HandleClick()
    {
        // Convert mouse position on screen into a ray into the 3D world
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
        {
            // Player clicks something with a collider
            GameObject clicked = hitInfo.collider.gameObject;

            // Check if its a battery slot
            if (clicked.TryGetComponent(out BatterySlot slot))
            {
                Debug.Log($"[InputManager] Clicked on a Slot: {slot.slotType}");

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnSlotClicked(slot.slotType);
                } else
                {
                    Debug.LogWarning("No GameManager instance found in scene");
                }
                return;
            }

            // Check if its a battery
            if (clicked.TryGetComponent(out BatteryObject battery))
            {
                Debug.Log($"[InputManager] Clicked on a Battery: {battery.batteryType}");

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnBatteryClicked(battery.batteryType);
                } else
                {
                    Debug.LogWarning("No GameManager instance found in scene");
                }
                return;
            }

            // Check if its an item
            if (clicked.TryGetComponent(out ItemObject item))
            {
                Debug.Log($"[InputManager] Clicked on an Item: {item.itemID}");

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnItemClicked(item.itemID);
                } else
                {
                    Debug.LogWarning("No GameManager instance found in scene");
                }
                return;
            }
        }
    }
}
