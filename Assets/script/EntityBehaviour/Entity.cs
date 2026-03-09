using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("实体属性")]
    [SerializeField] protected int maxHealth;//最大血量
    [SerializeField] protected int currentHealth;//血量
    [SerializeField] protected int maxMana;//最大蓝条
    [SerializeField] protected int currentMana;//蓝条
    [SerializeField] protected float invulnerableTime;//无敌时间
    [SerializeField] protected int physicalDefence;//物抗
    [SerializeField] protected int magicalDefence;//法抗
    [SerializeField] protected int evasion;//闪避，和Attack类的命中一起计算
    [SerializeField] protected float speed;

    [Header("实体状态")]
    [SerializeField] protected bool isInvulnerable;//无敌，防止被叠叠乐一巴掌打死
    protected bool isDrop;//是否掉落物品

    //实体控件
    public GameObject OriginalPrefab { get; set; }
    public System.Action OnDeath { get; set; }

    protected SpriteRenderer entitySpriteRenderer;
    protected Transform playerTransform;
    protected Animator entityAnimator;
    protected Rigidbody2D entityRigidbody;
    protected Collider2D entityCollider;
    protected Vector2 MovementInput;

    //掉落物品
    [SerializeField] private List<ItemDrop> drops = new List<ItemDrop>();
    protected virtual void Awake()
    {
        InitializeEntityComponent();
        InitializeStatus();
    }

    protected virtual void Start() { }
    protected virtual void Update() { }
    protected void InitializeEntityComponent()
    {
        entitySpriteRenderer = GetComponent<SpriteRenderer>();
        entityAnimator = GetComponent<Animator>();
        entityRigidbody = GetComponent<Rigidbody2D>();
        entityCollider = GetComponent<Collider2D>();
    }
    protected void InitializeStatus()//初始化实体状态
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        isInvulnerable = false;
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    public virtual void ResetEntity()
    {
        currentHealth = maxHealth;
    }
    private int DamageFormula(DamageInfo damageInfo)
    {
        DamageAttributes attackerDamageAttributes = new AttackerDamageAttributes(damageInfo.PhysicalDamage, damageInfo.MagicalDamage,damageInfo.Accuracy);
        DamageAttributes defenderDamageAttributes = new DefenderDamageAttributes(physicalDefence, magicalDefence, evasion);
        return DamageCalculator.CalculateDamage(attackerDamageAttributes, defenderDamageAttributes);
    }
    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        if (isInvulnerable) return;
        int finalDamage = DamageFormula(damageInfo);
        currentHealth = Mathf.Max(currentHealth - finalDamage, 0);
        Debug.Log(gameObject.name + "受到了" + finalDamage + "点伤害，血量为" + currentHealth);
        if (finalDamage != 0)
        {
            HitEvent();
            ExecuteHitFeedback(damageInfo);
        }
        else
        {
            GameManager.Instance.ShowMiss(transform);
        }
        StartCoroutine(nameof(InvulnerableCoroutine));
        if(currentHealth <= 0)
        {
            Die();
            Drop();
            return;
        }
    }
    protected virtual void Drop()
    {
        if (isDrop) return;
        float randomValue = Random.value;
        float accumulatedRate = 0f;

        foreach (var drop in drops)
        {
            accumulatedRate += drop.rate;
            if (randomValue <= accumulatedRate)
            {
                if (drop.item != null)
                {
                    drop.item.Drop(transform.position);
                }
                isDrop = true;
                return;
            }
        }
    }
    private void ExecuteHitFeedback(DamageInfo damageInfo)//受击反馈
    {
        Camera.main.transform.DOShakePosition(0.1f, 0.25f);
        if (damageInfo.ApplyRepel)
            ApplyRepel(damageInfo.AttackerPosition, damageInfo.RepelForce);
        if(damageInfo.ApplyShock)
            ApplyShock(damageInfo.ShockTime);
    }
    private void ApplyRepel(Vector2 attackerPosition, float repelForce)
    {
        Vector2 repelDirection = (entityRigidbody.position - attackerPosition).normalized;
        Vector2 AxisX = new Vector2(1f, 0f);
        repelDirection *=AxisX;
        entityRigidbody.AddForce(repelDirection * repelForce, ForceMode2D.Impulse);
    }
    protected virtual void Die()
    {
        StartCoroutine(Dying());
    }
    private IEnumerator Dying()
    {
        yield return new WaitForSeconds(2f);
        OnDeath?.Invoke();
    }
    IEnumerator InvulnerableCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerableTime);
        isInvulnerable = false;
    }
    protected virtual void HitEvent(){  }
    protected virtual void ApplyShock(float shockTime) { }
    public virtual void Heal(int value) => currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
    public virtual void RestoreMana(int value) => currentMana = Mathf.Clamp(currentMana + value, 0, maxMana);
    public bool ManaCost(int value)
    {
        if(currentMana<value)
            return false;
        currentMana = Mathf.Clamp(currentMana - value, 0, maxMana);
        return true;
    }
    private void OnTriggerEnter2D(Collider2D obj)
    {
        UpdateSortingOrder(obj);
    }
    private void UpdateSortingOrder(Collider2D other)
    {
        if (!other.TryGetComponent<SpriteRenderer>(out var otherRenderer)) return;
        entitySpriteRenderer.sortingOrder = otherRenderer.sortingOrder +
            (transform.position.y > other.transform.position.y ? -1 : 1);
    }
    public int ReturnCurrentHealth() => currentHealth;
    public int ReturnMaxHealth() => maxHealth;
}
public struct DamageInfo
{
    public int PhysicalDamage;
    public int MagicalDamage;
    public Vector2 AttackerPosition;
    public bool ApplyRepel;
    public float RepelForce;
    public bool ApplyShock;
    public float ShockTime;
    public int Accuracy;

    public DamageInfo(int physicalDamage,int magicalDamage,int accuracy, Vector2 attackerPosition, bool applyRepel = false, float repelForce = 0,bool applyShock = false, float shockTime = 0)
    {
        PhysicalDamage = physicalDamage;
        MagicalDamage = magicalDamage;
        AttackerPosition = attackerPosition;
        ApplyRepel = applyRepel;
        RepelForce = repelForce;
        ApplyShock = applyShock;
        ShockTime = shockTime;
        Accuracy = accuracy;
    }
}
public class DamageCalculator
{
    // 配置常量
    private const float CritDamageRatio = 1.5f;  // 暴击伤害倍率
    private const float BaseCritCap = 0.3f;     // 基础暴击率上限
    private const int CritSmoothFactor = 300;   // 暴击曲线平滑系数
    private const int MinDamage = 1;            // 最低保底伤害

    /// <summary>
    /// 综合伤害计算方法
    /// </summary>
    /// <param name="attacker">攻击方属性</param>
    /// <param name="defender">防御方属性</param>
    /// <returns>伤害结果(整数)或Miss</returns>
    public static int CalculateDamage(DamageAttributes attacker, DamageAttributes defender)
    {
        // 阶段1：命中判定
        float hitRate = CalculateHitRate(attacker.accuracy, defender.evasion);
        if (Random.value >= hitRate)
        {
            return 0; // 未命中
        }

        // 阶段2：暴击判定
        bool isCrit = CheckCritical(attacker.accuracy, defender.evasion);

        // 阶段3：伤害计算
        int totalDamage = CalculateTotalDamage(attacker, defender, isCrit);

        return totalDamage;
    }

    /// <summary>
    /// 计算命中率（0~1范围）
    /// </summary>
    private static float CalculateHitRate(int accuracy, int evasion)
    {
        // 公式：命中 / (命中 + 闪避 + 1)
        // +1防止除零错误，确保分母不为零
        return (float)accuracy / (accuracy + evasion + 1);
    }

    /// <summary>
    /// 暴击判定逻辑
    /// </summary>
    private static bool CheckCritical(int accuracy, int evasion)
    {
        int delta = accuracy - evasion;
        if (delta <= 0) return false; // 未达暴击触发阈值

        // 暴击率计算公式
        float critChance = (delta / (float)(delta + CritSmoothFactor)) * BaseCritCap;
        return Random.value < critChance;
    }

    /// <summary>
    /// 总伤害计算核心
    /// </summary>
    private static int CalculateTotalDamage(DamageAttributes atk, DamageAttributes def, bool isCrit)
    {
        // 物理伤害计算（双曲函数）
        float physicalDamage = def.physicalDefense > 0 ?
            Mathf.Pow(atk.physicalAtk, 2) / (atk.physicalAtk + def.physicalDefense) :
            atk.physicalAtk;

        // 法术伤害计算（双曲函数）
        float magicalDamage = def.magicalDefense > 0 ?
            Mathf.Pow(atk.magicalAtk, 2) / (atk.magicalAtk + def.magicalDefense) :
            atk.magicalAtk;

        // 合计基础伤害
        float total = physicalDamage + magicalDamage;

        // 暴击伤害加成
        if (isCrit) total *= CritDamageRatio;

        // 结果处理：保底伤害 + 整数化
        return Mathf.Max(MinDamage, Mathf.FloorToInt(total));
    }
}

/// <summary>
/// 战斗属性数据容器
/// </summary>
public class DamageAttributes
{
    public int physicalAtk;    // 物理攻击
    public int magicalAtk;     // 法术攻击
    public int accuracy;       // 命中值
    public int physicalDefense;// 物理防御
    public int magicalDefense; // 法术防御
    public int evasion;        // 闪避值
    
}
public class AttackerDamageAttributes : DamageAttributes {
    public AttackerDamageAttributes(int physicalAtk, int magicalAtk, int accuracy)
    {
        this.physicalAtk = physicalAtk;
        this.magicalAtk = magicalAtk;
        this.accuracy = accuracy;
    }
}
public class DefenderDamageAttributes : DamageAttributes
{
    public DefenderDamageAttributes(int physicalDefence, int magicalDefense, int evasion)//防守方
    {
        this.physicalDefense = physicalDefence;
        this.magicalDefense = magicalDefense;
        this.evasion = evasion;
    }
}