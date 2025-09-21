using UnityEngine;

[CreateAssetMenu(menuName = "Survivors/Ally")]
public class SurvivorStats : ScriptableObject
{
    [SerializeField] public GameObject model;
    [SerializeField] public int HP;
    [SerializeField] public int FaceTargetSpeed;
    [SerializeField] public int FOV;
    [SerializeField] public float ShootRate;
    [SerializeField] public float DetectionRadius;
    [SerializeField] public float Speed;
    [SerializeField] public float Acceleration;
    [SerializeField] public GameObject Bullet;
}
