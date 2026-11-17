using UnityEngine;


public class DoorOpener : MonoBehaviour
{
    public GameObject door; // The door GameObject to be opened
    public AudioSource doorAS;
    public AudioClip doorOpen;
    public AudioClip doorClose;
    public float openAngle = 100f; // The angle to open the door
    public float openSpeed = 3f; // The speed at which the door opens

    private bool isOpen;
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    void Start()
    {
        // Try to find the door GameObject if it is not assigned in the inspector
        if (door == null)
        {
            Transform doorTransform = transform.Find("Door");
            if (doorTransform != null)
            {
                door = doorTransform.gameObject;
            }   
            if (door == null)
            {
                Debug.LogError("Door GameObject not found.");
                return;
            }
        }
        // Try to find the AudioSource if it is not assigned in the inspector
        if (doorAS == null)
        {
            Transform doorASTransform = transform.Find("DoorSFX");
            if (doorASTransform != null)
            {
                doorAS = doorASTransform.GetComponent<AudioSource>();
            }
            if (doorAS == null)
            {
                Debug.LogError("Door AudioSource not found.");
                return;
            }
        }
        if (doorOpen == null)
        {
            Debug.LogError("Door open AudioClip not assigned.");
            return;
        }
        if (doorClose == null)
        {
            Debug.LogError("Door close AudioClip not assigned.");
            return;
        }
        closedRotation = door.transform.rotation;
        targetRotation = closedRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player (or another specified object)
        if (other.CompareTag("Player")) // Make sure the player GameObject has the tag "Player"
        {
            // Direction from door to player
            Vector3 toPlayer = other.transform.position - door.transform.position;

            // Positive = player is in front of the door, Negative = player is behind the door
            float dotProduct = Vector3.Dot(door.transform.forward, toPlayer.normalized);

            // Open the door away from the player
            float angle = dotProduct > 0 ? openAngle : -openAngle;
            targetRotation = Quaternion.Euler(0, angle, 0) * closedRotation;
            isOpen = true;

            // Play the door opening sound
            doorAS.clip = doorOpen;
            doorAS.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting the trigger is the player (or another specified object)
        if (other.CompareTag("Player")) // Make sure the player GameObject has the tag "Player"
        {
            if (isOpen)
            {
                isOpen = false;
                targetRotation = closedRotation;
                // Play the door closing sound
                doorAS.clip = doorClose;
                doorAS.Play();
            }
        }
    }

    void Update()
    {
        // Smoothly rotate the door towards the target rotation
        door.transform.rotation = Quaternion.Slerp(door.transform.rotation, targetRotation, Time.deltaTime * openSpeed);
    }
}
