using UnityEngine;

public class HeadbuttChecker : MonoBehaviour
{
    public bool touchingCeiling = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            touchingCeiling = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            touchingCeiling = false;
        }
    }


}
