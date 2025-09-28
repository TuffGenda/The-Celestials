using System.Collections;
using UnityEngine;

public class effect : MonoBehaviour
{
    [SerializeField] float secondsTillDelete;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(deletion());
    }

    IEnumerator deletion()
    {
        yield return new WaitForSeconds(secondsTillDelete);
        Destroy(gameObject);
    }
}
