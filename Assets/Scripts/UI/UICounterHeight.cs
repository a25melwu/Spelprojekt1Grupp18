using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class UICounterHeight : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text timerText;
    private TimerManager timerManager;

    void Awake()
    {
        if (timerText == null)
        {
            timerText = GetComponentInChildren<TMPro.TMP_Text>();
        }
        
        timerManager = FindFirstObjectByType<TimerManager>();
    }
    void OnEnable()
    {
        if (timerManager == null) //FIXED: this is added if time manager is under dontdestroyonload
        {
            timerManager = TimerManager.Instance;
        }
        
        if (timerManager != null && timerText != null)
        {
            timerText.text = timerManager.GetFormattedTime();
        }
        
    }
    
}
