using System.Collections;
using UnityEngine;

namespace Snake
{
    public class snakeBlackBoard : Blackboard
    {
        public Rigidbody2D rb { get; set; }
        public Animator ani { get; set; }
        public Transform transform { get; set; }
        public Transform playerTransform { get; set; }
        public Vector2 MovementInput { get; set; }
        public float DectectingRadius { get; set; }
        public float AttackRadius { get; set; }
        public float ActionInterval { get; set; }
        public float speed { get; set; }
        public bool isHit { get; set; }
        public bool isDead { get; set; }
    }
    public static class AnimHash
    {
        public static readonly int Awake = Animator.StringToHash("Awake");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Hit = Animator.StringToHash("Hit");
        public static readonly int Death = Animator.StringToHash("Death");
        public static readonly int Ready = Animator.StringToHash("Ready");
        public static readonly int Move = Animator.StringToHash("Move");
        //public static readonly int Shock = Animator.StringToHash("Shock");
    }
    public class IdleState : IState
    {
        private FSM fsm;
        private snakeBlackBoard bb;
        public IdleState(FSM fsm)
        {
            this.fsm = fsm;
            bb = fsm.blackboard as snakeBlackBoard;
        }

        public void OnEnter()
        {
            bb.rb.velocity = Vector2.zero;
            bb.ani.SetBool(AnimHash.Awake, false);
            bb.ani.SetBool(AnimHash.Ready, true);
        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {

        }
    }
    public class AttackState : IState
    {
        private float AttackTimer;
        private FSM fsm;
        private snakeBlackBoard bb;
        public AttackState(FSM fsm)
        {
            this.fsm = fsm;
            bb = fsm.blackboard as snakeBlackBoard;
        }
        public void OnEnter()
        {
            bb.ani.SetBool(AnimHash.Awake, false);
            bb.ani.SetBool(AnimHash.Ready,true);
            AttackTimer = 0;
        }

        public void OnExit()
        {
            AttackTimer = 0;
        }

        public void OnUpdate()
        {
            bb.rb.velocity = Vector2.zero;
            AttackTimer += Time.deltaTime;
            if (AttackTimer > bb.ActionInterval)
            {
                bb.ani.SetTrigger(AnimHash.Attack);
                AttackTimer = 0;
            }
        }
    }
    public class MoveState : IState
    {
        private FSM fsm;
        private snakeBlackBoard bb;
        private float idleTimer;
        public MoveState(FSM fsm)
        {
            this.fsm = fsm;
            bb = fsm.blackboard as snakeBlackBoard;
        }
        public void OnEnter()
        {
            bb.ani.SetBool(AnimHash.Awake, true);
            bb.ani.SetBool(AnimHash.Move, true);
            Flip();
            idleTimer = 0;
        }

        public void OnExit()
        {
            bb.ani.SetBool(AnimHash.Move, false);
        }

        public void OnUpdate()
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= bb.ActionInterval)
            {
                bb.ani.SetBool(AnimHash.Move, true);
                Vector2 dir = bb.playerTransform.position - bb.transform.position;
                bb.MovementInput = dir.normalized;
                Move();
            }
        }
        void Move()
        {
            if (bb.MovementInput.magnitude > 0.1f && bb.speed >= 0)
            {
                bb.rb.velocity = bb.MovementInput * bb.speed;
                if (bb.MovementInput.x < 0)
                {
                    bb.transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (bb.MovementInput.x > 0)
                {
                    bb.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else
            {
                bb.rb.velocity = Vector2.zero;
            }
        }
        void Flip()
        {
            Vector2 dir = (bb.playerTransform.position - bb.transform.position).normalized;
            if (dir.x < 0)
            {
                bb.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            if (dir.x > 0)
            {
                bb.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
    public class HitState : IState
    {
        private FSM fsm;
        private snakeBlackBoard bb;
        public HitState(FSM fsm)
        {
            this.fsm = fsm;
            bb = fsm.blackboard as snakeBlackBoard;
        }
        public void OnEnter()
        {
            bb.ani.SetTrigger(AnimHash.Hit);
        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {

        }
    }
    public class DeathState : IState
    {
        private FSM fsm;
        private snakeBlackBoard bb;
        public DeathState(FSM fsm)
        {
            this.fsm = fsm;
            bb = fsm.blackboard as snakeBlackBoard;
        }
        public void OnEnter()
        {
            bb.ani.SetBool(AnimHash.Death, true);
            bb.rb.simulated = false;
        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {

        }
    }
    public class Snake : Enemy
    {
        private snakeBlackBoard blackboard;
        private FSM fsm;
        private BehaviorTree aiTree;
        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            InitializeBlackboard();
            InitializeStateMachine();
            BuildBehaviorTree();
        }
        protected override void Update()
        {
            if (Time.frameCount % 2 == 0) // 降低行为树更新频率
            {
                aiTree.Tick();
            }
            fsm.OnUpdate();
        }
        public override void ResetEntity()
        {
            blackboard.isDead = false;
            blackboard.isHit = false;
            blackboard.rb.simulated = true;
            currentHealth = maxHealth;
        }
        private void BuildBehaviorTree()
        {
            /* 行为树结构：
             * Selector (root)
             * ├── Sequence [死亡]
             * │   ├── CheckDeath
             * │   └── SwitchState(Death)
             * ├── Sequence [受击]
             * │   ├── CheckHit
             * │   └── SwitchState(Hit)
             * ├── Sequence [攻击]
             * │   ├── CheckDistance(atradius)
             * │   └── SwitchState(Attack)
             * ├── Sequence [移动]
             * │   ├── CheckDistance(radius)
             * │   └── SwitchState(Move)
             * └── SwitchState(Idle) [默认]
             */
            var root = new SelectorNode(blackboard);

            // 死亡分支
            var deathSeq = new SequenceNode(blackboard);
            deathSeq.AddChild(new CheckDeathNode(blackboard));
            deathSeq.AddChild(new SwitchStateNode(
                blackboard, fsm, StateType.Death));

            // 受击分支
            var hitSeq = new SequenceNode(blackboard);
            hitSeq.AddChild(new CheckHitNode(blackboard));
            hitSeq.AddChild(new SwitchStateNode(
                blackboard, fsm, StateType.Hit));

            // 攻击分支
            var attackSeq = new SequenceNode(blackboard);
            attackSeq.AddChild(new CheckDistanceNode(
                blackboard, blackboard.AttackRadius));
            attackSeq.AddChild(new SwitchStateNode(
                blackboard, fsm, StateType.Attack));

            // 移动分支
            var moveSeq = new SequenceNode(blackboard);
            moveSeq.AddChild(new CheckDistanceNode(
                blackboard, blackboard.DectectingRadius));
            moveSeq.AddChild(new SwitchStateNode(
                blackboard, fsm, StateType.Move));

            // 构建树结构
            root.AddChild(deathSeq);
            root.AddChild(hitSeq);
            root.AddChild(attackSeq);
            root.AddChild(moveSeq);
            root.AddChild(new SwitchStateNode(
                blackboard, fsm, StateType.Idle));

            aiTree = new BehaviorTree(blackboard, root);
        }
        private void InitializeStateMachine()
        {
            fsm = new FSM(blackboard);
            fsm.AddState(StateType.Idle, new IdleState(fsm));
            fsm.AddState(StateType.Move, new MoveState(fsm));
            fsm.AddState(StateType.Attack, new AttackState(fsm));
            fsm.AddState(StateType.Hit, new HitState(fsm));
            fsm.AddState(StateType.Death, new DeathState(fsm));
            fsm.SwitchState(StateType.Idle);
        }

        private void InitializeBlackboard()
        {
            blackboard = new snakeBlackBoard();
            blackboard.DectectingRadius = DetectingRadius;
            blackboard.AttackRadius = AttackRadius;
            blackboard.ActionInterval = 1f;
            blackboard.speed = speed;

            blackboard.ani = entityAnimator;
            blackboard.transform = transform;
            blackboard.playerTransform = playerTransform;
            blackboard.MovementInput = MovementInput;
            blackboard.rb = entityRigidbody;
        }
        protected override void HitEvent()//受到伤害时触发
        {
            blackboard.isHit = true;
            StartCoroutine(ResetHitFlag());
        }
        protected override void Die()
        {
            base.Die();
            blackboard.isDead = true;
            Drop();
        }
        void OnAttack()//攻击动画关键帧调用
        {
            DamageInfo dInfo = new DamageInfo(RangeAttackPhysicalDamage, RangeAttackMagicalDamage, RangeAttackAccuracy, transform.position);
            GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>().TakeDamage(dInfo);
        }
        private IEnumerator ResetHitFlag()
        {
            yield return new WaitForSeconds(0.1f);
            blackboard.isHit = false;
        }
        void GoIdle()//Attack、Hit动画结束时触发该事件
        {
            if (fsm.IsCurrentState(StateType.Attack))
            {
                fsm.SwitchState(StateType.Idle);
            }
        }
    }
    /// <summary>
    /// 距离检测条件节点
    /// </summary>
    public class CheckDistanceNode : ConditionNode
    {
        private float checkDistance;
        private bool shouldBeLess;

        public CheckDistanceNode(
            snakeBlackBoard bb,
            float distance,
            bool lessThan = true
        ) : base(bb)
        {
            checkDistance = distance;
            shouldBeLess = lessThan;
        }

        protected override bool CheckCondition()
        {
            snakeBlackBoard bb = blackboard as snakeBlackBoard;
            if (bb.playerTransform == null) return false;

            float actualDistance = Vector2.Distance(
                bb.playerTransform.position,
                bb.transform.position
            );

            return shouldBeLess ?
                (actualDistance <= checkDistance) :
                (actualDistance > checkDistance);
        }
    }

    /// <summary>
    /// 受击状态检测节点
    /// </summary>
    public class CheckHitNode : ConditionNode
    {
        public CheckHitNode(snakeBlackBoard bb) : base(bb) { }

        protected override bool CheckCondition()
        {
            snakeBlackBoard bb = blackboard as snakeBlackBoard;
            return bb.isHit;
        }
    }

    /// <summary>
    /// 死亡状态检测节点
    /// </summary>
    public class CheckDeathNode : ConditionNode
    {
        public CheckDeathNode(snakeBlackBoard bb) : base(bb) { }

        protected override bool CheckCondition()
        {
            snakeBlackBoard bb = blackboard as snakeBlackBoard;
            return bb.isDead;
        }
    }
}