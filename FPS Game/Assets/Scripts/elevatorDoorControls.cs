using System.Collections;
using UnityEngine;

public class elevatorDoorControls : MonoBehaviour
{
    [SerializeField] Transform doorL;
    [SerializeField] Transform doorR;
    [SerializeField] Transform destinationL;
    [SerializeField] Transform destinationR;

    public bool posSet;
    public bool doorsOpen;
    public bool doorsClosed;

    Vector3 startingPosL;
    Vector3 startingPosR;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posSet = false;
        doorsOpen = false;
    }

    public void setStartingPos()
    {
        startingPosL = doorL.localPosition;
        startingPosR = doorR.localPosition;
        posSet = true;
    }

    public IEnumerator open(int seconds, int speed)
    {
        yield return new WaitForSeconds(seconds);
        doorL.localPosition = Vector3.MoveTowards(doorL.localPosition, destinationL.localPosition, speed * Time.deltaTime);
        doorR.localPosition = Vector3.MoveTowards(doorR.localPosition, destinationR.localPosition, speed * Time.deltaTime);

        if (doorL.localPosition == destinationL.localPosition && doorR.localPosition == destinationR.localPosition)
        {
            doorsOpen = true;
        }
    }

    public IEnumerator close(int seconds, int speed)
    {
        yield return new WaitForSeconds(seconds);
        doorL.localPosition = Vector3.MoveTowards(doorL.localPosition, startingPosL, speed * Time.deltaTime);
        doorR.localPosition = Vector3.MoveTowards(doorR.localPosition, startingPosR, speed * Time.deltaTime);

        if (doorL.localPosition == startingPosL && doorR.localPosition == startingPosR)
        {
            doorsOpen = false;
            doorsClosed = true;
        }
    }
}
