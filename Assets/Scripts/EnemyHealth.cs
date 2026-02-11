using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHp = 50f;
    float hp;

    void Awake()
    {
        hp = maxHp;
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0f)
            Destroy(gameObject);
    }
}
