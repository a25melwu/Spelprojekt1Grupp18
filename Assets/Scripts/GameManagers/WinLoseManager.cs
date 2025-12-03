using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseManager : MonoBehaviour
{
    public GameObject playerDeathParticle;
    

    private void Awake()
    {
        
    }
    
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
        StartCoroutine(nameof(LoseGameTimer));
    }

    IEnumerator LoseGameTimer()
    {
        if (playerDeathParticle != null)
            Instantiate(playerDeathParticle, FindFirstObjectByType<PlatformerMovement>().transform.position, Quaternion.identity);
        Destroy(FindFirstObjectByType<PlatformerMovement>().transform.parent.gameObject);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        WinGame();
    //    }
    //}
    
    

}
