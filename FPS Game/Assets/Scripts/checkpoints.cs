using UnityEngine;
using System.Collections;

public class checkpoints : MonoBehaviour
{
    [SerializeField] Renderer model;

    Color colorOriginal;

    private void Start()
    {
        colorOriginal = model.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gamemanager.instance.playerSpawnPOS.transform.position != transform.position)
        {
            gamemanager.instance.playerSpawnPOS.transform.position = transform.position;
            StartCoroutine(checkpointFeedback());
        }
    }

    IEnumerator checkpointFeedback()
    {
        model.material.color = Color.red;
        //gamemanager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        //gamemanager.instance.checkpointPopup.SetActive(true);
        model.material.color = colorOriginal;
    }
}
