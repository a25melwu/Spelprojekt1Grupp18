using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UICounterHeight : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text distanceText;
    private HeightManager heightManager;

    private string distanceFormat = "{0:0}m";

    void Awake()
    {
        if (distanceText == null)
        {
            distanceText = GetComponentInChildren<TMPro.TMP_Text>();
        }
        
        heightManager = FindFirstObjectByType<HeightManager>();
        
    }

    void OnEnable()
    {
        if (heightManager == null) //FIXED: this is added if time manager is under dontdestroyonload
        {
            heightManager = HeightManager.Instance;
        }

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (heightManager != null && distanceText != null)
        {
            distanceText.text = string.Format(distanceFormat, heightManager.DistanceToTopMeters);
        }
    }
}
