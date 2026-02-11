using System.Reflection;
using TMPro;
using UnityEngine;

public class HudHpText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private EnemyHealth target;

    // EnemyHealth 안의 hp/maxHp 같은 값을 "이름이 뭐든" 최대한 찾아서 표시해줌
    private FieldInfo hpField;
    private FieldInfo maxField;

    private void Reset()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Awake()
    {
        if (text == null) text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (target == null) target = FindObjectOfType<EnemyHealth>();
        CacheFields();
    }

    private void Update()
    {
        if (text == null) return;

        if (target == null)
        {
            target = FindObjectOfType<EnemyHealth>();
            CacheFields();
        }

        if (target == null)
        {
            text.text = "";
            return;
        }

        float hp = ReadHp();
        float max = ReadMaxHp();

        // max가 못 찾아지면 hp로라도 표시
        if (max <= 0) max = Mathf.Max(hp, 1);

        text.text = $"ENEMY HP  {Mathf.CeilToInt(hp)}/{Mathf.CeilToInt(max)}";
    }

    private void CacheFields()
    {
        hpField = null;
        maxField = null;

        if (target == null) return;

        var t = target.GetType();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        // 흔한 이름들 후보
        hpField  = t.GetField("hp", flags) 
                ?? t.GetField("Hp", flags)
                ?? t.GetField("currentHp", flags)
                ?? t.GetField("CurrentHp", flags)
                ?? t.GetField("health", flags)
                ?? t.GetField("Health", flags);

        maxField = t.GetField("maxHp", flags)
                ?? t.GetField("MaxHp", flags)
                ?? t.GetField("maxHP", flags)
                ?? t.GetField("MaxHP", flags);
    }

    private float ReadHp()
    {
        if (hpField == null) return 0;
        object v = hpField.GetValue(target);
        if (v is int i) return i;
        if (v is float f) return f;
        return 0;
    }

    private float ReadMaxHp()
    {
        if (maxField == null) return 0;
        object v = maxField.GetValue(target);
        if (v is int i) return i;
        if (v is float f) return f;
        return 0;
    }
}
