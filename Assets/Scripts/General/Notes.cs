using UnityEngine;
using UnityEditor;

public class Notes : MonoBehaviour
{

    [Header("Notes"), TextArea(2, 10)]
    [SerializeField] private string notes;
}
