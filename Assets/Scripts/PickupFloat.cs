using UnityEngine;

public class PickupFloat : MonoBehaviour
{
    public float moveDistance = 0.3f;   
    public float moveSpeed = 1f;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;

        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
