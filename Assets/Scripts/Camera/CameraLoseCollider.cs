using UnityEngine;

public class CameraLoseCollider : MonoBehaviour
{
    [SerializeField] private BoxCollider2D loseCollider;

    private void Start()
    {
        Camera cam = Camera.main;

        transform.position = new Vector2(cam.transform.position.x, cam.transform.position.y - cam.orthographicSize);

        float width = cam.orthographicSize * 2f * cam.aspect;
        loseCollider.size = new Vector2(width, loseCollider.size.y);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            WinLoseManager winLoseManager = FindFirstObjectByType<WinLoseManager>();
            
            if(winLoseManager != null)
            {
                winLoseManager.LoseGame();
            }
            else
            {
                Debug.LogError("No WinLoseManager found in scene! It is a prefab and you just need to drag it into the scene.");
            }
        }
    }



}
