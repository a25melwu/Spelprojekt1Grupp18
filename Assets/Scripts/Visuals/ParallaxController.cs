using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private float spriteHeight;
    private Transform mainCamera;
    [Header("Parallax Settings")]
    [Tooltip("Set Parallax Offset Effect: 0 = not moving, 1 = same as camera")]
    [SerializeField] private float parallaxEffect = 0.5f;
    
    private Vector3 initialCameraPosition;
    private Vector3 initialSpritePosition;
    private float offsetY; //track accumulated offset for wrapping
    private float cameraHeight;
    
    void Awake()
    {
        mainCamera = Camera.main.transform;
        cameraHeight = Camera.main.orthographicSize * 2f; //set camera height once
        
        initialCameraPosition = mainCamera.position; //store initial position of camera
        initialSpritePosition = transform.position; //store initial position of sprite
        
        spriteHeight = GetComponent<SpriteRenderer>().bounds.size.y; //calculate sprite length
        
    }

    void LateUpdate() //ensures parallax updates AFTER camera movement is complete in the frame
    {
        //calculate parallax movement
        float cameraTravelY = mainCamera.position.y - initialCameraPosition.y; //calculate camera movement
        float parallaxOffset = (cameraTravelY * parallaxEffect); //apply parallax effect to this sprite
        float targetY = initialSpritePosition.y + parallaxOffset + offsetY;
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z); //move the sprite
        
        //get camera bounds
        float cameraTop = mainCamera.position.y + (cameraHeight / 2);
        float cameraBottom = mainCamera.position.y - (cameraHeight / 2);
        
        //get sprite bounds
        float spriteTop = transform.position.y + (spriteHeight / 2);
        float spriteBottom = transform.position.y - (spriteHeight / 2);
        
        //infinite vertical scroll
        //check if sprite is out of camera view
        if (spriteTop < cameraBottom) //sprite is below camera
        {
            float newY = cameraTop + (spriteHeight / 2); //change position of sprite to be ABOVE camera
            offsetY += (newY - transform.position.y);
           //Debug.Log($"WRAP UP: Sprite wrapped to top");
        }
        
        else if (spriteBottom > cameraTop) //sprite is above camera
        {
            float newY = cameraBottom - (spriteHeight / 2); //change position of sprite to BELOW camera
            offsetY += (newY - transform.position.y);
            //Debug.Log($"WRAP DOWN: Sprite wrapped to bottom");
        }
        
    }
}
