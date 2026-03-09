using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Queen
{
    [Serializable]
    public class QueenBlackboard : Blackboard
    {
        [Header("战斗设置")]
        [SerializeField] public float idleTime;//攻击前摇
        [SerializeField] public float radius;

        public Animator ani;
        public Transform transform;
        public Transform playerTransform;
    }
    public static class AnimHash
    {
        public static readonly int Attack1 = Animator.StringToHash("attack1");
        public static readonly int Attack2 = Animator.StringToHash("attack2");
        public static readonly int Attack3 = Animator.StringToHash("attack3");
        public static readonly int Death = Animator.StringToHash("death");
    }
    public class IdleState : IState
    {
        private float idleTimer;
        private FSM fsm;
        private QueenBlackboard bb;
        private bool isHealthBarShow;
        public IdleState(FSM fsm)
        {
            this.fsm = fsm;
            bb = fsm.blackboard as QueenBlackboard;
        }
        public void OnEnter()
        {
            idleTimer = 0;
        }

        public void OnExit()
        {
            idleTimer = 0;
        }

        public void OnUpdate()
        {
            if (bb.transform != null && bb.playerTransform != null)
            {
                float distance = Vector2.Distance(bb.playerTransform.position, bb.transform.position);
                if (distance < bb.radius)
                {
                    if (!isHealthBarShow)
                    {
                        isHealthBarShow = true;
                        UIManager.Instance.GetPanel(UIConst.BossHealthBarPanel).SetActive(true);
                    }
                    idleTimer += Time.deltaTime;
                    if (idleTimer > bb.idleTime)
                    {
                        fsm.SwitchState(StateType.Attack);
                    }
                }
                else
                {
                    isHealthBarShow = false;
                    UIManager.Instance.GetPanel(UIConst.BossHealthBarPanel).SetActive(false);
                }

            }
        }
    }
    public class AttackState : IState
    {
        private FSM fsm;
        private QueenBlackboard bb;
        private int mode;
        public AttackState(FSM fsm)
        {
            this.fsm = fsm;
            bb = fsm.blackboard as QueenBlackboard;
        }
        public void OnEnter()
        {
            float distance = Vector2.Distance(bb.playerTransform.position, bb.transform.position);
            if (distance >= 6.5)
                mode = 2;
            else
            {
                float value = UnityEngine.Random.value;
                if (value <= 0.5f)
                    mode = 0;
                else mode = 1;
            }
            switch (mode)
            {
                case 0:
                    bb.ani.SetTrigger(AnimHash.Attack1);
                    fsm.SwitchState(StateType.Idle);
                    break;
                case 1:
                    bb.ani.SetTrigger(AnimHash.Attack2);
                    fsm.SwitchState(StateType.Idle);
                    break;
                case 2:
                    bb.ani.SetTrigger(AnimHash.Attack3);
                    fsm.SwitchState(StateType.Idle);
                    break;
            }
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
        private QueenBlackboard bb;
        public DeathState(FSM fsm)
        {
            this.fsm = fsm;
            bb = fsm.blackboard as QueenBlackboard;
        }
        public void OnEnter()
        {
            bb.ani.SetBool(AnimHash.Death, true);
        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {

        }
    }
    public class Queen : Enemy
    {
        private FSM fsm;
        private QueenBlackboard blackboard;
        public GameObject Queen_spur;
        protected override void Start()
        {
            base.Start();
            BossHealthBar.MaxHealth = maxHealth;
            BossHealthBar.CurrentHealth = currentHealth;
            InitializeBlackboard();

            UIManager.Instance.OpenPanel(UIConst.BossHealthBarPanel);
            SetupStateMachine();
        }
        protected override void Update()
        {
            fsm.OnUpdate();
        }
        private void SetupStateMachine()
        {
            fsm = new FSM(blackboard);
            fsm.AddState(StateType.Idle, new IdleState(fsm));
            fsm.AddState(StateType.Attack, new AttackState(fsm));
            fsm.AddState(StateType.Death, new DeathState(fsm));
            fsm.SwitchState(StateType.Idle);
        }
        private void InitializeBlackboard()
        {
            blackboard = new QueenBlackboard();
            blackboard.ani = entityAnimator;
            blackboard.transform = transform;
            blackboard.playerTransform = playerTransform;
            blackboard.radius = DetectingRadius;
            blackboard.idleTime = 6f;
        }
        protected override void HitEvent()
        {
            base.HitEvent();
            BossHealthBar.UpdateHealth(currentHealth);
        }
        protected override void Die()
        {
            fsm.SwitchState(StateType.Death);
            UIManager.Instance.ClosePanel(UIConst.BossHealthBarPanel);
            base.Die();

        }
        void spurSpringout()
        {
            Vector3 position = new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z);
            Instantiate(Queen_spur, position, Quaternion.identity);
        }
    }
}