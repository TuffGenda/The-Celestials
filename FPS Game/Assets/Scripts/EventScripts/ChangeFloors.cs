using UnityEngine;

public class ChangeFloors : MonoBehaviour
{
    bool inTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger && Input.GetButtonDown("Interact"))
        {
            gamemanager.instance.floorsMenu();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger && other.CompareTag("Player"))
        {
            inTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger && other.CompareTag("Player"))
        {
            inTrigger = false;
        }
    }
}
