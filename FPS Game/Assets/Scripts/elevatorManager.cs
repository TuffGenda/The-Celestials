using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class elevatorManager : MonoBehaviour
{
    [SerializeField] int speed;
    [SerializeField] int doorSpeed;
    [SerializeField] int secondsTillDoorsClose;

    [SerializeField] Transform elevator;
    [SerializeField] Transform destination;
    [SerializeField] GameObject doorControls;
    [SerializeField] GameObject floor;
    

    elevatorDoorControls controls;
    parentObjects parentScript;
    Vector3 startingPos;

    public bool atDestination;
    bool firstRun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPos = elevator.position;
        atDestination = false;
        firstRun = true;
        controls = doorControls.GetComponent<elevatorDoorControls>();
        parentScript = floor.GetComponent<parentObjects>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!atDestination && firstRun)
        {
            moveUp();
        }
        else if (atDestination && !controls.doorsOpen)
        {
            if (!controls.posSet)
            {
                controls.setStartingPos();
            }
            else
            {
                StartCoroutine(controls.open(secondsTillDoorsClose, doorSpeed));
            }
        }
        else if (!atDestination && parentScript.playerParented)
        {
            moveUp();
        }
    }

    public void moveUp()
    {
        StartCoroutine(move(destination.position, 0));
    }

    IEnumerator move(Vector3 target, int seconds)
    {
        yield return new WaitForSeconds(seconds);
        atDestination = false;
        elevator.position = Vector3.MoveTowards(elevator.position, target, speed * Time.deltaTime);

        if (elevator.position == destination.position)
        {
            atDestination = true;
            firstRun = false;
        }

        if (elevator.position == startingPos)
        {
            controls.doorsClosed = false;
        }
    }
}
