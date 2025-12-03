using UnityEngine;

public class BasicCameraMovement : MonoBehaviour
{
    [Tooltip("The offset of where the camera starts to move up if the player exceeds a certain height")]
    [SerializeField] private float cameraYSensitivityOffset = 3f;

    private Transform playerTransform;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = FindFirstObjectByType<PlatformerMovement>().transform;
        if(playerTransform == null) { Debug.LogError("Warning: The camera can't find the player!"); }
        else
        {
            //If we want the camera to be at the player at start: uncomment the following line of code
            //transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null) { return; }

        if (playerTransform.position.y > transform.position.y + cameraYSensitivityOffset)
        {
            SetCameraToCorrectPosition();
        }
    }


    public void SetCameraToCorrectPosition()
    {

        transform.position = new Vector3(transform.position.x, playerTransform.position.y - cameraYSensitivityOffset, transform.position.z);
    }
}
