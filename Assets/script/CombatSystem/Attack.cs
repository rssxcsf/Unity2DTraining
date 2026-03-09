using UnityEngine;

public class Attack : MonoBehaviour //挂在嵌套在实体的攻击对象下
{
    [SerializeField] protected bool isPlayerAttack;//是不是玩家的攻击，用来实现通用碰撞体攻击简化代码的，后面改Tag用
    [SerializeField] protected int physicalDamage;
    [SerializeField] protected int magicalDamage;
    [SerializeField] protected bool isRepel;//击退效果
    [SerializeField] protected int repelLevel;//击退等级
    [SerializeField] protected bool isShock;//眩晕效果
    [SerializeField] protected float shockTime;//眩晕时间
    [SerializeField] protected int accuracy;//命中
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        ExecuteAttack(other);
    }
    protected virtual void ExecuteAttack(Collider2D other)
    {
        Vector2 attackOrigin = transform.position;

        DamageInfo dInfo = new DamageInfo(
            physicalDamage,
            magicalDamage,
            accuracy,
            attackOrigin,
            isRepel,
            repelLevel,
            isShock,
            shockTime
        );

        const string PLAYER_TAG = "Player";
        const string ENEMY_TAG = "Enemy";

        string expectedTag = isPlayerAttack ? ENEMY_TAG : PLAYER_TAG;

        if (!other.CompareTag(expectedTag)) return;

        Entity targetEntity = other.GetComponent<Entity>();
        if (targetEntity == null)
        {
            Debug.LogError($"【战斗系统】{this}攻击{other}时未找到Entity组件（预期标签：{expectedTag}）");
            return;
        }
        targetEntity.TakeDamage(dInfo);
        OnTriggerEnterEvent();
    }
    protected virtual void OnTriggerEnterEvent() { }
    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
