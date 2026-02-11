using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float amplitude = 2f; // 좌우 이동 범위
    public float speed = 1.5f;   // 이동 속도

    Vector3 start;

    void Awake()
    {
        start = transform.position;
    }

    void Update()
    {
        transform.position = start + Vector3.right * (Mathf.Sin(Time.time * speed) * amplitude);
    }
}
