using UnityEngine;

public class jumpScare : MonoBehaviour
{
    [SerializeField] int speed;

    [SerializeField] Transform platform;
    [SerializeField] Transform destination;
    //[SerializeField] Collider triggerCollider;

    Vector3 startingPos;
    bool isTriggered = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPos = platform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered)
        {
            platform.position = Vector3.MoveTowards(platform.transform.position, destination.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerJumpScare();
        }
    }

    public void TriggerJumpScare()
    {
        isTriggered = true;
    }
}
