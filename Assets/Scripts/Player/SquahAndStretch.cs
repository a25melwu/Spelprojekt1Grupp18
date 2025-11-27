using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class SquahAndStretch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D playerRigidbody2D;
    [SerializeField] private Transform spriteToAffect;

    [Header("Stretch Settings")]
    [SerializeField] private float stretchMultiplier = 0.05f;
    [SerializeField] private float maxStretch = 0.35f;
    [SerializeField] private float returnToNormalSpeed = 30f;

    //Values
    private Vector3 originalScale;

    void Start()
    {
        originalScale = spriteToAffect.localScale;
    }

    [Obsolete]
    void Update()
    {
        float yVelocity = playerRigidbody2D.velocity.y;

        float fallStretch = Mathf.Clamp(-yVelocity * stretchMultiplier, 0, maxStretch);
        float riseStretch = Mathf.Clamp(yVelocity * stretchMultiplier, 0, maxStretch);
        float finalStretch = fallStretch + riseStretch;

        float stretchY = 1 + finalStretch;
        float stretchX = 1 - finalStretch * 0.5f;

        Vector3 target = new Vector3(
            originalScale.x * stretchX,
            originalScale.y * stretchY,
            1);

        spriteToAffect.localScale = Vector3.Lerp(spriteToAffect.localScale, target, Time.deltaTime * returnToNormalSpeed);
    }
}
