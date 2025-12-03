using UnityEngine;
using UnityEngine.SceneManagement;

public class AdminTools : MonoBehaviour
{
    [SerializeField] private GameObject[] teleporters;

    private PlatformerMovement playerMovement;

    private void Start()
    {
        Debug.LogWarning("You have the admin tools activated");

        playerMovement = FindFirstObjectByType<PlatformerMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (playerMovement != null)
                playerMovement.AddDoubleJump(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (playerMovement != null && teleporters[0] != null)
            {
                playerMovement.transform.position = teleporters[0].transform.position;
                FindFirstObjectByType<BasicCameraMovement>().SetCameraToCorrectPosition();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (playerMovement != null && teleporters[1] != null)
            {
                playerMovement.transform.position = teleporters[1].transform.position;
                FindFirstObjectByType<BasicCameraMovement>().SetCameraToCorrectPosition();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (playerMovement != null && teleporters[2] != null)
            {
                playerMovement.transform.position = teleporters[2].transform.position;
                FindFirstObjectByType<BasicCameraMovement>().SetCameraToCorrectPosition();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (playerMovement != null && teleporters[3] != null)
            {
                playerMovement.transform.position = teleporters[3].transform.position;
                FindFirstObjectByType<BasicCameraMovement>().SetCameraToCorrectPosition();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (playerMovement != null && teleporters[4] != null)
            {
                playerMovement.transform.position = teleporters[4].transform.position;
                FindFirstObjectByType<BasicCameraMovement>().SetCameraToCorrectPosition();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (playerMovement != null && teleporters[5] != null)
            {
                playerMovement.transform.position = teleporters[5].transform.position;
                FindFirstObjectByType<BasicCameraMovement>().SetCameraToCorrectPosition();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (playerMovement != null && teleporters[6] != null)
            {
                playerMovement.transform.position = teleporters[6].transform.position;
                FindFirstObjectByType<BasicCameraMovement>().SetCameraToCorrectPosition();
            }
        }
    }
}
