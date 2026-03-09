using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerBlackBoard : Blackboard
{
    public Rigidbody2D rb { get; private set; }
    public Animator ani { get; private set; }
    public Transform transform { get; private set; }
    public SkillManager skillManager { get; private set; }
    public Barrage playerBarrage1 { get; set; }
    public Barrage playerBarrage2 { get; set; }


    public float speed { get; set; }
    public float shockTime {  get; set; }

    // 状态参数
    public bool IsCombatSystemActive { get;set; }
    public bool IsGrounded { get; set; }
    public float MoveInputH { get; set; }
    public float MoveInputV { get; set; }
    public bool JumpPressed { get; set; }
    public bool AttackPressed { get; set; }
    public WorldItem ItemInRange { get; set; }
    public NpcDialogueSystem NpcInRange { get; set; }
    public bool SkillAPressed { get; set; }
    public bool SkillBPressed { get; set; }
    public bool SkillCPressed { get; set; }

    public void Initialize(Player player)
    {
        rb = player.GetComponent<Rigidbody2D>();
        ani = player.GetComponent<Animator>();
        transform = player.transform;
        skillManager = player.GetComponent<SkillManager>();
    }
}
/// <summary>
/// 动画参数哈希（提升性能）
/// </summary>
public static class PlayerAnimHash
{
    public static readonly int Run = Animator.StringToHash("Run");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int Hit = Animator.StringToHash("Hit");
    public static readonly int Shock = Animator.StringToHash("Shock");
    public static readonly int Death = Animator.StringToHash("Death");
    public static readonly int isAttacking = Animator.StringToHash("isAttacking");
    public static readonly int Combo = Animator.StringToHash("combo");
    public static readonly int Pick = Animator.StringToHash("Pick");
    public static readonly int Sliding = Animator.StringToHash("Sliding");
}
public class JumpHandler
{
    private readonly PlayerBlackBoard _bb;

    public JumpHandler(PlayerBlackBoard bb) => _bb = bb;

    public void HandleJump()
    {
        if (!_bb.JumpPressed) return;

        if (_bb.IsGrounded) ExecuteJump();
    }
    private void ExecuteJump()
    {
        _bb.ani.SetTrigger(PlayerAnimHash.Jump);
        _bb.rb.velocity = new Vector2(_bb.rb.velocity.x, 0);
    }
}

public abstract class PlayerState : IState
{
    protected readonly FSM Fsm;
    protected readonly PlayerBlackBoard BB;
    protected readonly JumpHandler JumpHandler;

    protected PlayerState(FSM fsm)
    {
        Fsm = fsm;
        BB = fsm.blackboard as PlayerBlackBoard;
        JumpHandler = new JumpHandler(BB);
    }
    /// <summary>
    /// 处理通用输入
    /// </summary>
    protected void HandleSkillLauncher()
    {
        if (BB.SkillAPressed)
        {
            if (BB.skillManager.IsSkillAReady())
                Fsm.SwitchState(StateType.Skill);
            else
                SubtitleManager.Instance.ShowTips("技能冷却中");
        }
        if (BB.SkillBPressed)
        {
            if (BB.skillManager.IsSkillBReady())
                Fsm.SwitchState(StateType.Skill);
            else
                SubtitleManager.Instance.ShowTips("技能冷却中");
        }
        if (BB.SkillCPressed)
        {
            if (BB.skillManager.IsSkillCReady())
                Fsm.SwitchState(StateType.Skill);
            else
                SubtitleManager.Instance.ShowTips("技能冷却中");
        }
    }
    protected void HandleCommonInput()
    {
        // 跳跃处理
        JumpHandler.HandleJump();
        
        //拾取物品or攻击检测
        if (BB.AttackPressed)
            {
            if (BB.NpcInRange != null)//对话优先
            {
                Fsm.SwitchState(StateType.Chat);
            }
            else if (BB.IsCombatSystemActive)
            {
                if (BB.ItemInRange != null)//拾取物品优先
                    Fsm.SwitchState(StateType.Pick);
                else if (BB.IsGrounded)
                    if (IsAnimationPlaying("Idle"))//闲置攻击
                        Fsm.SwitchState(StateType.Attack);
                    else if(IsAnimationPlaying("Run")&&Mathf.Abs(BB.MoveInputH)>0.8f)
                        Fsm.SwitchState(StateType.Sliding);//移动则滑铲
            }
        }
    }
    public bool IsAnimationPlaying(string animationName)
    {
        return BB.ani.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }
    public virtual void UpdatePhysics()
    {
        BB.IsGrounded = (!IsAnimationPlaying("Jump"));
    }
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnUpdate();
}

/// <summary>
/// 闲置状态
/// </summary>
public class IdleState : PlayerState
{
    public IdleState(FSM fsm) : base(fsm) { }
    public override void OnEnter() => BB.ani.SetBool(PlayerAnimHash.Run, false);

    public override void OnExit() { }
    public override void OnUpdate()
    {
        HandleSkillLauncher();
        HandleCommonInput();
        UpdatePhysics();

        // 移动检测
        if (Mathf.Abs(BB.MoveInputV) > 0.1f || Mathf.Abs(BB.MoveInputH) > 0.1f)
            Fsm.SwitchState(StateType.Move);
    }
}
/// <summary>
/// 移动状态（包含平滑移动和转向）
/// </summary>
public class MoveState : PlayerState
{
    public MoveState(FSM fsm) : base(fsm) { }

    public override void OnEnter() => BB.ani.SetBool(PlayerAnimHash.Run, true);

    public override void OnExit() => BB.ani.SetBool(PlayerAnimHash.Run, false);

    public override void OnUpdate()
    {
        HandleCommonInput();
        UpdatePhysics();

        // 平滑移动
        var velocityX = new Vector2(BB.MoveInputH * BB.speed, BB.rb.velocity.y);
        BB.rb.velocity = velocityX;
        var velocityY = new Vector2(BB.rb.velocity.x, BB.MoveInputV * BB.speed);
        BB.rb.velocity = velocityY;

        // 转向处理
        if (BB.MoveInputH > 0.1f) BB.transform.localEulerAngles = Vector3.zero;
        else if (BB.MoveInputH < -0.1f) BB.transform.localEulerAngles = new Vector3(0, 180, 0);

        // 返回闲置状态
        if (Mathf.Abs(BB.MoveInputH) < 0.1f && Mathf.Abs(BB.MoveInputV) < 0.1f)
            Fsm.SwitchState(StateType.Idle);
    }
}
public class SlidingState : PlayerState
{
    private float slideDuration = 0.6f; // 状态持续时间
    private float timer;
    public SlidingState(FSM fsm) : base(fsm) { }

    public override void OnEnter()
    {
        BB.ani.SetTrigger(PlayerAnimHash.Sliding);
        timer = 0f;
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= slideDuration)
        {
            Fsm.SwitchState(StateType.Idle);
        }
    }
}
    public class HitState : PlayerState
{
    public HitState(FSM fsm) : base(fsm) { }
    public override void OnEnter()
    {
        BB.ani.SetTrigger(PlayerAnimHash.Hit);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {

    }
}
public class ShockState : PlayerState
{
    private float shockTimer;
    public ShockState(FSM fsm) : base(fsm) { }
    public override void OnEnter()
    {
        BB.ani.SetBool(PlayerAnimHash.Shock, true);
        shockTimer = 0;
    }

    public override void OnExit()
    {
        BB.ani.SetBool(PlayerAnimHash.Shock, false);
    }

    public override void OnUpdate()
    {
        shockTimer += Time.deltaTime;
        if (shockTimer > BB.shockTime)
        {
            Fsm.SwitchState(StateType.Idle);
        }
    }
}
public class DeathState : PlayerState
{
    public DeathState(FSM fsm) : base(fsm) { }
    public override void OnEnter()
    {
        BB.ani.SetBool(PlayerAnimHash.Death, true);
        BB.rb.simulated = false;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {

    }
}
public class AttackState : PlayerState
{
    public AttackState(FSM fsm) : base(fsm) { }
    private int mode;
    public override void OnEnter()
    {
        BB.ani.SetBool(PlayerAnimHash.isAttacking, true);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        UpdatePhysics();
        if (BB.AttackPressed && BB.IsGrounded)
            BB.ani.SetTrigger(PlayerAnimHash.Combo);
    }
}
public class PickState : PlayerState
{
    public PickState(FSM fsm) : base(fsm) { }

    public override void OnEnter()
    {
        //执行动画
        BB.ani.SetTrigger(PlayerAnimHash.Pick);
        BB.ItemInRange.GetComponent<WorldItem>().PickUp();
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {

    }
}
public class ChatState : PlayerState
{
    public ChatState(FSM fsm) : base(fsm) { }

    public override void OnEnter()
    {
        BB.NpcInRange.Communicate();
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        if (BB.AttackPressed)
            if (!BB.NpcInRange.returnDialogBoxState())
                BB.NpcInRange.Communicate();
        if (BB.NpcInRange.returnDialogBoxState())
            Fsm.SwitchState(StateType.Idle);
    }
}
public class SkillState : PlayerState
{
    public SkillState(FSM fsm) : base(fsm) { }

    public override void OnEnter()
    {
        if (BB.SkillAPressed)
            BB.skillManager.LaunchSkillA();
        if(BB.SkillBPressed)
            BB.skillManager.LaunchSkillB();
        if (BB.SkillBPressed)
            BB.skillManager.LaunchSkillC();
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        
    }
}
    public class Player : Entity
{
    private FSM fsm;
    private PlayerBlackBoard blackboard;
    [SerializeField] private bool isCombatSystemActive;
    [SerializeField] private Barrage playerBarrage1;
    [SerializeField] private Barrage playerBarrage2;
    protected override void Start()
    {
        currentHealth = maxHealth;
        InitializeEntityComponent();
        InitializeBlackboard();
        SetupStateMachine();
        InitializeGUI();
        SubtitleManager.Instance.ShowSubtitles("休息的时间结束了...");
    }

    protected override void Update()
    {
        UpdateInput();
        UpdateStateBar();
        fsm.OnUpdate();
    }
    private void SetupStateMachine()
    {
        //状态机初始化
        fsm = new FSM(blackboard);
        fsm.AddState(StateType.Idle, new IdleState(fsm));
        fsm.AddState(StateType.Move, new MoveState(fsm));
        fsm.AddState(StateType.Sliding, new SlidingState(fsm));
        fsm.AddState(StateType.Hit, new HitState(fsm));
        fsm.AddState(StateType.Shock, new ShockState(fsm));
        fsm.AddState(StateType.Death, new DeathState(fsm));
        fsm.AddState(StateType.Attack, new AttackState(fsm));
        fsm.AddState(StateType.Skill, new SkillState(fsm));
        fsm.AddState(StateType.Pick, new PickState(fsm));
        fsm.AddState(StateType.Chat, new ChatState(fsm));
        fsm.SwitchState(StateType.Idle);
    }
    private void InitializeBlackboard()
    {
        blackboard = new PlayerBlackBoard();
        blackboard.Initialize(this);//初始化玩家碰撞体刚体
        blackboard.speed = speed;
        blackboard.IsCombatSystemActive = isCombatSystemActive;
        blackboard.playerBarrage1 = playerBarrage1;
        blackboard.playerBarrage2 = playerBarrage2;
    }
    private void InitializeGUI()
    {
        StatePanel.MaxHealth = maxHealth;
        StatePanel.CurrentHealth = currentHealth;
        StatePanel.MaxMana = maxMana;
        StatePanel.CurrentMana = currentMana;
        UIManager.Instance.OpenPanel(UIConst.PlayerHealthBarPanel);
    }
    void UpdateStateBar()
    {
        StatePanel.UpdateMana(currentMana);
    }
    protected override void HitEvent()
    {
        StatePanel.UpdateHealth(currentHealth);
        fsm.SwitchState(StateType.Hit);
    }
    public override void Heal(int value)
    {
        base.Heal(value);
        StatePanel.UpdateHealth(currentHealth);
    }
    protected override void Die()
    {
        fsm.SwitchState(StateType.Death);
        //打开死亡复活界面
    }
    protected override void ApplyShock(float shockTime)
    {
        blackboard.shockTime = shockTime;
        fsm.SwitchState(StateType.Shock);
    }
    private void UpdateInput()
    {
        blackboard.MoveInputH = Input.GetAxis("Horizontal");
        blackboard.MoveInputV = Input.GetAxis("Vertical");
        blackboard.JumpPressed = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.X);
        blackboard.AttackPressed = Input.GetMouseButtonDown(0);
        blackboard.SkillAPressed = Input.GetKeyDown(KeyCode.U);
        blackboard.SkillBPressed = Input.GetKeyDown(KeyCode.I);
        blackboard.SkillCPressed = Input.GetKeyDown(KeyCode.O);
    }
    private void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.CompareTag("Item"))
        {
            blackboard.ItemInRange = obj.GetComponent<WorldItem>();
        }
        if (obj.gameObject.CompareTag("Npc"))
        {
            blackboard.NpcInRange = obj.GetComponent<NpcDialogueSystem>();
        }
    }
    private void OnTriggerExit2D(Collider2D obj)
    {
        if (obj.gameObject.CompareTag("Item"))
        {
            blackboard.ItemInRange = null;
        }
        if (obj.gameObject.CompareTag("Npc"))
        {
            blackboard.NpcInRange = null;
        }
    }
    void AttackPhaseOne()
    {
        GameObject.Instantiate(blackboard.playerBarrage1, blackboard.transform.position + new Vector3(0f, 0, 0), blackboard.transform.rotation);
    }
    void AttackPhaseTwo()
    {
        blackboard.ani.SetTrigger(PlayerAnimHash.Combo);
        GameObject.Instantiate(blackboard.playerBarrage2, blackboard.transform.position + new Vector3(0f,0, 0), blackboard.transform.rotation);
    }
    void Sliding()
    {
        Vector2 direction = transform.right;
        entityRigidbody.AddForce(direction * 30f, ForceMode2D.Impulse);
    }
    void OnDeathEnd()
    {
        UIManager.Instance.OpenPanel("GameOverPanel");
    }
    void OnAttackEnd()
    {
        blackboard.ani.SetBool(PlayerAnimHash.isAttacking, false);
        fsm.SwitchState(StateType.Idle);
    }
    void GoIdle()
    {
        fsm.SwitchState(StateType.Idle);
    }
    void OpenDeathMenu()
    {
        UIManager.Instance.OpenPanel("GameOverPanel");
    }
}