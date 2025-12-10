using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private int playerDoubleJumpsSaved = 0;

    public List<float> collectedFeathersID = new();

    public float idOfCheckpointToSpawnAt = -1; 

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    public void SavePlayerDoubleJump(int amountToSetTo)
    {
        playerDoubleJumpsSaved = amountToSetTo;
    }

    public int GetDoubleJumpsPlayerShouldStartWith()
    {
        return playerDoubleJumpsSaved;
    }

    //Calls every time the player dies as well, since we reload the scene
    [System.Obsolete]
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPlayerToCorrectSpawnPosition();
    }

    [System.Obsolete]
    public void SetPlayerToCorrectSpawnPosition()
    {
        Checkpoint[] checkPoints = FindObjectsOfType<Checkpoint>();
        Checkpoint checkPointToSpawnAt = null;

        for (int i = 0; i < checkPoints.Length; i++)
        {
            if(checkPoints[i].checkpointId == idOfCheckpointToSpawnAt)
            {
                checkPointToSpawnAt = checkPoints[i];
            }
        }

        if (checkPointToSpawnAt == null) return;

        GameObject player = FindFirstObjectByType<PlatformerMovement>().gameObject;
        player.transform.position = checkPointToSpawnAt.spawnPos.position;
    }

}
