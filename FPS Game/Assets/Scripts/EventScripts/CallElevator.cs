using UnityEngine;

public class CallElevator : MonoBehaviour
{
    [SerializeField] GameObject elevator;
    [SerializeField] GameObject elevatorWall;
    

    elevatorManager elevatorScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elevatorScript = elevator.GetComponent<elevatorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (elevatorScript.atDestination)
        {
            elevatorWall.SetActive(false);
        }
        else
        {
            elevatorWall.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger && !elevatorScript.atDestination && other.CompareTag("Player"))
        {
            elevatorScript.moveUp();
        }
    }
}
