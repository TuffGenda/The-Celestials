using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using static UnityEditor.Progress;

// Event script created by Tuff Genda.
public class Event : MonoBehaviour
{
    enum objectWindow
    {
        Object, Window, Both, Cure
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
        else if (eventType == objectWindow.Both)
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
        else if (eventType == objectWindow.Cure)
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
            else if (eventType == objectWindow.Both)
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
            else if (eventType == objectWindow.Cure)
            {
                levelManager.instance.CheckEnding();

                if (levelManager.instance.goodEnding)
                {
                    foreach (var item in objectsToShow)
                    {
                        if (item.name == "REAL Cure" || item.name == "Cure Created")
                        {
                            item.SetActive(true);
                        }
                    }
                }
                else
                {
                    foreach (var item in objectsToShow)
                    {
                        if (item.name == "Cure" || item.name == "Cure Created")
                        {
                            item.SetActive(true);
                        }
                    }
                }

                foreach (GameObject item in objectsToHide)
                {
                    item.SetActive(false);
                }
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
