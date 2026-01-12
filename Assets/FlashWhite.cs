using System.Collections;
using UnityEngine;

public class FlashWhite : MonoBehaviour
{
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private SpriteRenderer sr;

    public float flashDuration = 0.2f;
    public Color flashColor = Color.black;

    private Color originalColor;
    private bool isChangingColor = false;

    void Awake()
    {
        originalColor = sr.color;
    }

    public void ChangeToMaxChargeColor()
    {
        if (isChangingColor == true) return;
        isChangingColor = true;
        StartCoroutine(ChangeToFullyChargedColor());
    }

    private IEnumerator ChangeToFullyChargedColor()
    {
        float time = 0f;

        while (time < flashDuration)
        {
            float t = time / flashDuration;
            float curveValue = animationCurve.Evaluate(t);
            time += Time.deltaTime;
            sr.color = Color.Lerp(originalColor, flashColor, curveValue);
            yield return null;
        }
        sr.color = originalColor;
        isChangingColor = false;
    }
}
