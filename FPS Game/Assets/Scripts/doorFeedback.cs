using UnityEngine;

public class doorFeedback : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gamemanager.instance != null)
            {
                gamemanager.instance.gameActionText.text = "This needs a key...";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gamemanager.instance != null)
            {
                gamemanager.instance.gameActionText.text = "";
            }
        }
    }
}
