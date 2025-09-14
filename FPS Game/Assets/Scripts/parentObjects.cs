using UnityEngine;

public class parentObjects : MonoBehaviour
{
    public bool playerParented;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = transform;
            playerParented = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = null;
            playerParented = false;
        }
    }
}
