using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private int playerDoubleJumpsSaved = 0;

    public List<float> collectedFeathersID = new();


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



}
