using UnityEngine;
using UnityEngine.UI;

public class CanvasTouchScript : MonoBehaviour
{
    private Color startColor;
    private Color opacityDown;

    public Image imageToGetColorFrom;

    private void Start()
    {
        startColor = imageToGetColorFrom.color;
        opacityDown = startColor;
        opacityDown.a = 0.5f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (imageToGetColorFrom.color != opacityDown)
            {
                imageToGetColorFrom.color = opacityDown;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (imageToGetColorFrom.color != startColor)
            {
                imageToGetColorFrom.color = startColor;
            }
        }
    }

}
