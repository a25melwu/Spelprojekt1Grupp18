using UnityEngine;

public class BackgroundFader : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;

    private Transform playerPos;

    public SpriteRenderer[] spritesToFadeOut;

    private void Start()
    {
        playerPos = FindFirstObjectByType<PlatformerMovement>().gameObject.transform;
    }

    private void Update()
    {
        float fadeAmount = 1f;

        float t = Mathf.InverseLerp(bottomPoint.position.y, topPoint.position.y, playerPos.position.y);
        fadeAmount = 1f - t;

        foreach (SpriteRenderer sr in spritesToFadeOut)
        {
            sr.color = new Color(1f, 1f, 1f, fadeAmount);
        }
    }
}
