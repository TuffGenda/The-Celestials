using System.Collections;
using UnityEngine;

public class elevatorDoorControls : MonoBehaviour
{
    [SerializeField] Transform doorL;
    [SerializeField] Transform doorR;
    [SerializeField] Transform destinationL;
    [SerializeField] Transform destinationR;
    [SerializeField] AudioClip openSound;
    [SerializeField] GameObject text;

    public bool posSet;
    public bool doorsOpen;
    public bool doorsClosed;
    bool audioDB = false;

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
        Destroy(text);
        doorL.localPosition = Vector3.MoveTowards(doorL.localPosition, destinationL.localPosition, speed * Time.deltaTime);
        doorR.localPosition = Vector3.MoveTowards(doorR.localPosition, destinationR.localPosition, speed * Time.deltaTime);
        if (!audioDB) {
            audioDB = true;
            gamemanager.instance.playerScript.plrSoundSource.PlayOneShot(openSound, 0.4f);
            gamemanager.instance.playerScript.sendActionText("");

            switch (levelManager.instance.currentLevel) { 
                case 2:
                    gamemanager.instance.playerScript.sendActionText("Floor 1 - General Paitent Ward");
                    break;
                case 3:
                    gamemanager.instance.playerScript.sendActionText("Floor 2 - Sick Paitent Ward");
                    break;
                case 4:
                    gamemanager.instance.playerScript.sendActionText("Floor 3 - Injured Paitent Ward");
                    break;
                case 5:
                    gamemanager.instance.playerScript.sendActionText("Floor 4 - Emergency Room");
                    break;
                case 6:
                    gamemanager.instance.playerScript.sendActionText("Floor 5 - Director's Office");
                    break;
                case 7:
                    gamemanager.instance.playerScript.sendActionText("Floor 0 - Basement");
                    break;
            }
        }
        if (doorL.localPosition == destinationL.localPosition && doorR.localPosition == destinationR.localPosition)
        {
            doorsOpen = true;
        }
    }
}
