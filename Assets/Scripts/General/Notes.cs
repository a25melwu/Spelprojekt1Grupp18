using UnityEngine;
using UnityEditor;

public class Notes : MonoBehaviour
{

    [Header("Notes"), TextArea(2, 5)]
    [SerializeField] private string notes;
}
