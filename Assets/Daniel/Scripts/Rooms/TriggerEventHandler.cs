using UnityEngine;

public class TriggerEventHandler : MonoBehaviour
{
    private RoomEventManager roomEventManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            roomEventManager = GetComponentInParent<RoomEventManager>();
            if (roomEventManager != null)
            {
                roomEventManager.OnPlayerEnterRoom();
                gameObject.SetActive(false);
            }
            else
            {

            }
        }
    }
}
