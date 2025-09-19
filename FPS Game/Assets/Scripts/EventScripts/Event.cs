using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// Event script created by Tuff Genda.
public class Event : MonoBehaviour
{
    enum objectWindow
    {
        Object, Window, both
    }

    [SerializeField] objectWindow eventType;
    [SerializeField] List<GameObject> objectsToShow;
    [SerializeField] List<GameObject> objectsToHide;
    [SerializeField] GameObject gameWindowToShow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (eventType == objectWindow.Object)
        {
            foreach (GameObject item in objectsToShow)
            {
                item.SetActive(false);
            }

            foreach (GameObject item in objectsToHide)
            {
                item?.SetActive(true);
            }
        }
        else if (eventType == objectWindow.both)
        {
            foreach (GameObject item in objectsToShow)
            {
                item.SetActive(false);
            }

            foreach (GameObject item in objectsToHide)
            {
                item?.SetActive(true);
            }

            gamemanager.instance.stateUnpause();
            gameWindowToShow.SetActive(false);
        }
        else
        {
            gamemanager.instance.stateUnpause();
            gameWindowToShow.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger && other.CompareTag("Player"))
        {
            if (eventType == objectWindow.Object)
            {
                foreach (GameObject item in objectsToShow)
                {
                    item.SetActive(true);
                }

                foreach (GameObject item in objectsToHide)
                {
                    item.SetActive(false);
                }
            }
            else if (eventType == objectWindow.both)
            {
                foreach (GameObject item in objectsToShow)
                {
                    item.SetActive(true);
                }

                foreach (GameObject item in objectsToHide)
                {
                    item.SetActive(false);
                }

                gamemanager.instance.statePause();
                gamemanager.instance.menuActive = gameWindowToShow;
                gamemanager.instance.menuActive.SetActive(true);
            }
            else
            {
                gamemanager.instance.statePause();
                gamemanager.instance.menuActive = gameWindowToShow;
                gamemanager.instance.menuActive.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(gameObject, 0);
    }
}
