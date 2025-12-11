using System.Collections.Generic;
using UnityEngine;

public class ParallaxControllerDuplicate : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("Set Parallax Offset Effect: 0 = not moving, 1 = same as camera")]
    [SerializeField] private float parallaxEffect = 0.5f;

    [Header("Child Sprites")] 
    [SerializeField] private Transform topChild;
    [SerializeField] private Transform bottomChild;
    private SpriteRenderer childRenderer;
    private float spriteHeight;
    private Transform mainCamera;
    private Vector3 initialCameraPosition;
    private float cameraHeight;
    
    void Awake()
    {
        mainCamera = Camera.main.transform;
        cameraHeight = Camera.main.orthographicSize * 2f; //set camera height once
        initialCameraPosition = mainCamera.position; //store initial position of camera

        if (topChild == null || bottomChild == null)
        {
            Debug.Log("ParallaxControllerDuplicate : assign both child sprites in the inspector!");
            enabled = false;
            return;
        }
        
        childRenderer = topChild.GetComponent<SpriteRenderer>();
        if (childRenderer != null) //get sprite height from one of the children
        {
            spriteHeight = GetVisualSpriteHeight();
            
        }
        else
        {
            Debug.Log("ParallaxControllerDuplicate : child has no spriterenderer!");
            enabled = false;
            return;
        }

        PositionChildren();
        UpdateParallax();
       
    }
    
    void LateUpdate() //ensures parallax updates AFTER camera movement is complete in the frame
    {
        UpdateParallax();
    }
    private void PositionChildren() //position children relative to parent
    {
        topChild.localPosition = new Vector3(0, spriteHeight, 0);
        bottomChild.localPosition = new Vector3(0, -spriteHeight, 0);
    }

    private void UpdateParallax()
    {
        //calculate parallax movement of PARENT
        float cameraTravelY = mainCamera.position.y - initialCameraPosition.y; //calculate camera movement
        float parallaxOffset = (cameraTravelY * parallaxEffect); //apply parallax effect to this sprite
        transform.position = new Vector3(transform.position.x, parallaxOffset, transform.position.z); //move main sprite
        
        //get camera bounds
        float cameraTop = mainCamera.position.y + (cameraHeight * 0.5f);
        float cameraBottom = mainCamera.position.y - (cameraHeight * 0.5f);

        WrapChild(topChild, cameraTop, cameraBottom);
        WrapChild(bottomChild, cameraTop, cameraBottom);
    }

    private void WrapChild(Transform child, float cameraTop, float cameraBottom)
    {
        //get children bounds
        float childTop = child.position.y + (spriteHeight * 0.5f);
        float childBottom = child.position.y - (spriteHeight * 0.5f);
        
        //infinite vertical scroll
        //check if main sprite is out of camera view
        if (childTop < cameraBottom) // if child top sprite is below camera, set this child above the other child
        {
            Transform otherChild = (child == topChild) ?  bottomChild : topChild;
            child.position = new Vector3(child.position.x, otherChild.position.y + spriteHeight, child.position.z);
            
        }
        else if (childBottom > cameraTop) //if child bottom sprite is above camera, set this child below the other child
        {
           Transform otherChild = (child == topChild) ? bottomChild : topChild;
           child.position = new Vector3(child.position.x, otherChild.position.y - spriteHeight, child.position.z);
        }
        
    }

    private float GetVisualSpriteHeight() //complements the overlapBuffer
    {
        if (childRenderer == null || childRenderer.sprite == null) return 1f;
        Sprite sprite = childRenderer.sprite;
        
        float rectHeight = sprite.rect.height /sprite.pixelsPerUnit; //rect height (cropped in sprite editor)

        float boundsHeight = childRenderer.bounds.size.y; //bounds (rendered, after cropping)
        
        return Mathf.Max(rectHeight, boundsHeight); //for no gaps, use the largest value of above
    }
}
