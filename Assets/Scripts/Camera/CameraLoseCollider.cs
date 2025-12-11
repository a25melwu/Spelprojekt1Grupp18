using UnityEngine;

public class CameraLoseCollider : MonoBehaviour
{
    [Header("Variables")]
    [Tooltip("The amount the collider is under the camera edge")]
    [SerializeField] private float yOffset = 3f;

    [Header("References")]
    [SerializeField] private BoxCollider2D loseCollider;
    private PlayerSFX playerSFX;

    private void Start()
    {
        //Set the colliders position to be at the edge of the screen - this way it will be correct even if the player plays on a weird screen
        Camera cam = Camera.main;
        transform.position = new Vector2(cam.transform.position.x, cam.transform.position.y - cam.orthographicSize - yOffset);
        float width = cam.orthographicSize * 2f * cam.aspect;
        loseCollider.size = new Vector2(width, loseCollider.size.y);

        playerSFX = FindFirstObjectByType<PlayerSFX>(); //get playerSFX player
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
            
            playerSFX?.PlayOutOfBoundsFallSound(); //added here instead of in winlosemanager or both hurt and outofbounds sounds are playing
        }
    }



}
