using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform spawnPos;
    private bool hasBeenTriggered = false;
    public float checkpointId;

    private void Awake()
    {
        checkpointId = transform.position.y;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!hasBeenTriggered)
            {
                hasBeenTriggered = true;
                FindFirstObjectByType<SaveManager>().idOfCheckpointToSpawnAt = checkpointId;
                if (FindFirstObjectByType<SaveManager>() == null)
                    Debug.Log("ONO");

            }
        }
    }

}
