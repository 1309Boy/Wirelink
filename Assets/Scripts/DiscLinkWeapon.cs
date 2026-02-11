using System.Collections.Generic;
using UnityEngine;

public class DiscLinkWeapon : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public GameObject discPrefab;
    public LineRenderer line;

    [Header("Layers")]
    public LayerMask wallLayer;
    public LayerMask enemyLayer;

    [Header("Rules")]
    public float maxDistance = 50f;
    public float vanishDelay = 1f;   // 연결 1초 후 소멸
    public float refillDelay = 2f;   // 연결 2초 후 2발 지급

    [Header("Damage")]
    public float dps = 20f;
    public float hitThickness = 0.25f;

    enum State { Ready, LinkedWaiting, EmptyWaiting }
    State state = State.Ready;

    int ammo = 2;
    readonly List<Transform> placed = new();
    float linkTime = -999f;

    void Start()
    {
        SetLineVisible(false);
    }

    void Update()
    {
        HandleTimers();

        if (Input.GetMouseButtonDown(0))
            TryPlaceDisc();

        // 라인 유지 중이면 업데이트 + 딜
        if (placed.Count == 2 && state != State.EmptyWaiting)
        {
            UpdateLine();
            DealDamageOverLine();
        }
        else
        {
            SetLineVisible(false);
        }
    }

    void HandleTimers()
    {
        if (state == State.LinkedWaiting && Time.time >= linkTime + vanishDelay)
        {
            ClearPlacedDiscs();
            SetLineVisible(false);
            state = State.EmptyWaiting;
        }

        if (state == State.EmptyWaiting && Time.time >= linkTime + refillDelay)
        {
            ammo = 2;
            state = State.Ready;
        }
    }

    void TryPlaceDisc()
    {
        if (state != State.Ready) return;
        if (ammo <= 0) return;

        if (!TryGetAttachPoint(out Vector2 attachPoint))
            return;

        SpawnDisc(attachPoint);
        ammo--;

        if (placed.Count == 2)
        {
            linkTime = Time.time;
            state = State.LinkedWaiting;
        }
    }

    bool TryGetAttachPoint(out Vector2 point)
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

       // 1) 벽 직접 클릭 (OverlapPoint)
        var col = Physics2D.OverlapPoint(mouseWorld, wallLayer);
        if (col != null)
        {
            point = col.ClosestPoint(mouseWorld);
            return true;
        }


        // 2) 허공 클릭 → 플레이어 기준 방향으로 가장 가까운 벽
        if (player == null)
        {
            point = default;
            return false;
        }

        Vector2 playerPos = player.position;
        Vector2 dir = (mouseWorld - playerPos).normalized;

        var hit = Physics2D.Raycast(playerPos, dir, maxDistance, wallLayer);
        if (hit.collider != null)
        {
             point = hit.point;
             return true;
        }


        point = default;
        return false;
    }

    void SpawnDisc(Vector2 at)
    {
        if (discPrefab == null) return;
        var disc = Instantiate(discPrefab, at, Quaternion.identity);
        placed.Add(disc.transform);
    }

    void ClearPlacedDiscs()
    {
        for (int i = 0; i < placed.Count; i++)
            if (placed[i] != null) Destroy(placed[i].gameObject);

        placed.Clear();
        SetLineVisible(false);
    }

    void UpdateLine()
    {
        if (line == null) return;
        SetLineVisible(true);
        line.positionCount = 2;
        line.SetPosition(0, placed[0].position);
        line.SetPosition(1, placed[1].position);
    }

    void DealDamageOverLine()
    {
        Vector2 a = placed[0].position;
        Vector2 b = placed[1].position;

        Vector2 delta = b - a;
        float dist = delta.magnitude;
        if (dist <= 0.001f) return;

        Vector2 dir = delta / dist;

        // 두께 있는 선 판정: 캡슐 캐스트
        var hits = Physics2D.CapsuleCastAll(
            (a + b) * 0.5f,
            new Vector2(dist, hitThickness),
            CapsuleDirection2D.Horizontal,
            Vector2.SignedAngle(Vector2.right, dir),
            Vector2.zero,
            0f,
            enemyLayer
        );

        float damage = dps * Time.deltaTime;
        foreach (var h in hits)
        {
            var hp = h.collider.GetComponentInParent<EnemyHealth>();
            if (hp != null) hp.TakeDamage(damage);
        }
    }

    void SetLineVisible(bool on)
    {
        if (line != null) line.enabled = on;
    }
}
