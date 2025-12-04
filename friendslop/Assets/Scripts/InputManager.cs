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
                //GameManager.OnSlotClicked(slot.slotType);
                Debug.Log($"Clicked on a Slot: {slot.slotType}");
                return;
            }

            // Check if its a battery
            if (clicked.TryGetComponent(out BatteryObject battery))
            {
                //GameManager.OnBatteryClicked(battery.batteryType);
                Debug.Log($"Clicked on a Battery: {battery.batteryType}");
                return;
            }

            // Check if its an item
            if (clicked.TryGetComponent(out ItemObject item))
            {
                //GameManager.OnItemClicked(item.itemID);
                Debug.Log($"Clicked on an Item: {item.itemID}");
                return;
            }
        }
    }
}
