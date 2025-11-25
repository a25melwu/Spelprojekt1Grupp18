using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UpdateValue : MonoBehaviour
{
    public TMPro.TMP_Text text;
    public Slider slider;
    private int percent;

    public void Start()
    {
        ValueToPercent();
        text.text = percent + "%";
    }
    public void Update()
    {
        ValueToPercent();
        text.text = percent + "%";
    }

    private int ValueToPercent()
    {
        percent = (int)(slider.value * 100);
        return percent;
    }
    
}
