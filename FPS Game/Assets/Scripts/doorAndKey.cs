using UnityEngine;
using System.Collections;
using Unity.AI.Navigation;

public class doorAndKey : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject keyObject;
    [SerializeField] GameObject doorObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (keyObject != null)
            {
                Destroy(keyObject);
            }
            if (doorObject != null)
            {
                Destroy(doorObject);
            }
            gamemanager.instance.playerScript.sendActionText("You heard a door open!");
        }
    }

    
}
