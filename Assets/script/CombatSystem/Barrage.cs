using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Barrage : Attack
{
    [Header("弹幕类攻击属性")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private bool isChasingPlayer;
    [SerializeField] private bool isPenetrate;
    [SerializeField] private float liveTime;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator animator;
    private float liveTimer;
    private bool HitTarget;
    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // 查找玩家对象
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("未找到玩家对象！");
            Destroy(gameObject); // 没有玩家时自毁
        }
    }
    protected override void Update()
    {
        HandleBarrageMovement();
        LifeTimeElapse();
    }
    void LifeTimeElapse()
    {
        liveTimer += Time.deltaTime;
        if (liveTimer > liveTime)
        {
            PlayExplosionAni();
            Destroy(gameObject, 0.2f);
        }
    }
    void HandleBarrageMovement()
    {
        if (!HitTarget)
        {
            if (isChasingPlayer)
                ChasingPlayer();
            else
                DefaultDirection();
        }
        else
            rb.velocity = Vector3.zero;
    }
    void DefaultDirection()
    {
        Vector2 moveDirection = transform.right;
        rb.velocity = moveDirection * moveSpeed;
    }
    void ChasingPlayer()
    {
        if (playerTransform == null) return;
        // 计算移动和旋转
        HandleRotation();
        HandleMovement();
    }
    void HandleRotation()
    {
        // 获取方向向量（从子弹指向玩家）
        Vector2 direction = (Vector2)playerTransform.position - rb.position;
        direction.Normalize();

        // 计算目标旋转角度（使用Atan2计算Z轴旋转）
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // 平滑旋转（使用刚体保证物理系统兼容性）
        float currentAngle = rb.rotation;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotateSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newAngle);
    }

    void HandleMovement()
    {
        // 获取当前正方向（transform.up）
        Vector2 moveDirection = transform.up;

        // 使用刚体移动（避免直接修改position）
        rb.velocity = moveDirection * moveSpeed;

        /* 备选移动方案：适用于非物理移动
        transform.Translate(
            moveDirection * moveSpeed * Time.deltaTime, 
            Space.World
        );*/
    }
    protected override void OnTriggerEnterEvent()
    {
        if (!isPenetrate)
        {
            PlayExplosionAni();
            HitTarget = true;
            Destroy(gameObject, 0.2f);
        }
    }
    protected virtual void PlayExplosionAni()
    {
        animator.SetTrigger(BarrageHash.Explosion);
    }
}
public static class BarrageHash
{
    public static readonly int Explosion = Animator.StringToHash("Explosion");
}
