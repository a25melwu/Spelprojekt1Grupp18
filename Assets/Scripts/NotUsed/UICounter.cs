using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UICounter : MonoBehaviour
{
    public TMPro.TMP_Text counterText;

    private int counter = 0;

    public UnityEvent onComparedAndTrue;

    public void Start()
    {
        
    }

    public void Update()
    {
        counterText.text = counter.ToString();
    }

    public void AddToCounter(int add)
    {
        counter += add;
        counterText.text = counter.ToString();
    }

    public void SetCounter(int set)
    {
        counter = set;
        counterText.text = counter.ToString();
    }

    public void CompareCounter(int other)
    {
        if (other == counter)
        {
            onComparedAndTrue.Invoke();
        }
    }
}
