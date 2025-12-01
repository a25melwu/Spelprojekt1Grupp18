using UnityEngine;
using UnityEngine.SceneManagement;

public class AdminTools : MonoBehaviour
{

    private void Start()
    {
        Debug.LogWarning("You have the admin tools activated");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
