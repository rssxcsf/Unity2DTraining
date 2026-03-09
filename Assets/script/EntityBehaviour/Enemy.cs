using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Enemy : Entity
{
    [SerializeField] public string EnemyTag;

    //基本属性
    [SerializeField] protected int RangeAttackPhysicalDamage; //非碰撞体范围攻击伤害
    [SerializeField] protected int RangeAttackMagicalDamage;
    [SerializeField] protected int RangeAttackAccuracy;
    [SerializeField] protected float DetectingRadius;
    [SerializeField] protected float AttackRadius;
    protected virtual void OnEnable()
    {
        ResetEntity();
    }
    protected override void Die()
    {
        EventManager.TriggerEnemyKilled(EnemyTag);
        LocalData.Instance.PlayerInfo.Earn(10);
        LocalData.Instance.Save();
        base.Die();
    }
    protected virtual void RangeAttack(float radius) { }
    protected virtual void RangeAttack(float radius, LayerMask playerLayer)//范围攻击，好像有bug暂时没用
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius, playerLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider != null)
            {
                Player pc;
                pc = hitCollider.GetComponent<Player>();
                if (pc != null)
                {
                    DamageInfo dInfo = new DamageInfo(RangeAttackPhysicalDamage, RangeAttackMagicalDamage, RangeAttackAccuracy, entityCollider.transform.position);
                    pc.TakeDamage(dInfo);
                    Debug.Log(hitCollider);
                }
                else
                {
                    Debug.Log("找不到Player");
                }

            }
        }
    }
}
