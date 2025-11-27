using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseManager : MonoBehaviour
{

    //Called when the player gets to the top
    //Reloads the scene
    public void WinGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Called when the player dies
    //Reloads the scene
    public void LoseGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            WinGame();
        }
    }
}
