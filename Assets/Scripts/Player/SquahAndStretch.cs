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

    [Header("Jump Anticipation Settings")]
    [SerializeField] private float squashMultiplier = 0.05f;
    [SerializeField] private float maxSquash = 0.35f;
    [SerializeField] private AnimationCurve squashCurve;
    [SerializeField] private float squashDuration = 2f;

    //Values
    private Vector3 originalScale;
    public float squashTimer;
    public bool isAnticipating;

    void Start()
    {
        originalScale = spriteToAffect.localScale;
    }

    [Obsolete]
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            squashTimer = 0;
            isAnticipating = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isAnticipating = false;
        }

        if (isAnticipating)
        {
            squashTimer += Time.deltaTime;
            float t = squashTimer / squashDuration; //t will always be between 0 and 1

            if (t >= 1f)
            {
                t = 1f;
            }

            //Scale the sprite
            float squashValue = squashCurve.Evaluate(t);
            spriteToAffect.localScale = new Vector3(originalScale.x * squashValue, originalScale.y * (2f - squashValue), originalScale.z);

            //Change position so it doesnt look like the sprite is floating

            float offset = (originalScale.y - spriteToAffect.localScale.y);
            spriteToAffect.localPosition = new Vector3(0, -offset * 0.5f, 0);
        }
        else
        {
            spriteToAffect.localScale = Vector3.Lerp(spriteToAffect.localScale, originalScale, Time.deltaTime * returnToNormalSpeed);
            spriteToAffect.localPosition = Vector3.Lerp(spriteToAffect.localPosition, new Vector3(0, 0, 0), Time.deltaTime * returnToNormalSpeed);




            float yVelocity = playerRigidbody2D.velocity.y;

            float fallStretch = Mathf.Clamp(-yVelocity * stretchMultiplier, 0, maxStretch);
            float riseStretch = Mathf.Clamp(yVelocity * stretchMultiplier, 0, maxStretch);
            float finalStretch = fallStretch + riseStretch;

            float stretchY = 1 + finalStretch;
            float stretchX = 1 - finalStretch * 0.5f;

            Vector3 targetShape = new Vector3(originalScale.x * stretchX, originalScale.y * stretchY, 1);

            spriteToAffect.localScale = Vector3.Lerp(spriteToAffect.localScale, targetShape, Time.deltaTime * returnToNormalSpeed);
        }




    }
}
