// region 行为树基础框架 ------------------------------------------------
using System.Collections.Generic;

/// <summary>
/// 节点执行状态
/// </summary>
public enum NodeStatus
{
    Success, // 执行成功
    Failure, // 执行失败
    Running  // 执行中
}

/// <summary>
/// 行为树节点基类
/// </summary>
public abstract class BTNode
{
    protected Blackboard blackboard;

    protected BTNode(Blackboard bb)
    {
        blackboard = bb;
    }

    public abstract NodeStatus Execute();
    public virtual void OnReset() { }
}

/// <summary>
/// 组合节点基类（可包含子节点）
/// </summary>
public abstract class CompositeNode : BTNode
{
    protected List<BTNode> children = new List<BTNode>();

    protected CompositeNode(Blackboard bb) : base(bb) { }

    public void AddChild(BTNode node)
    {
        children.Add(node);
    }
}

/// <summary>
/// 顺序节点（全部子节点成功才算成功）
/// </summary>
public class SequenceNode : CompositeNode
{
    private int currentIndex;

    public SequenceNode(Blackboard bb) : base(bb)
    {
        currentIndex = 0;
    }

    public override NodeStatus Execute()
    {
        while (currentIndex < children.Count)
        {
            var status = children[currentIndex].Execute();

            if (status == NodeStatus.Running)
                return NodeStatus.Running;

            if (status == NodeStatus.Failure)
            {
                Reset();
                return NodeStatus.Failure;
            }

            currentIndex++;
        }

        Reset();
        return NodeStatus.Success;
    }

    private void Reset() => currentIndex = 0;
}

/// <summary>
/// 选择节点（任一子节点成功即成功）
/// </summary>
public class SelectorNode : CompositeNode
{
    private int currentIndex;

    public SelectorNode(Blackboard bb) : base(bb)
    {
        currentIndex = 0;
    }

    public override NodeStatus Execute()
    {
        while (currentIndex < children.Count)
        {
            var status = children[currentIndex].Execute();

            if (status == NodeStatus.Running)
                return NodeStatus.Running;

            if (status == NodeStatus.Success)
            {
                Reset();
                return NodeStatus.Success;
            }

            currentIndex++;
        }

        Reset();
        return NodeStatus.Failure;
    }

    private void Reset() => currentIndex = 0;
}

/// <summary>
/// 条件节点基类
/// </summary>
public abstract class ConditionNode : BTNode
{
    protected ConditionNode(Blackboard bb) : base(bb) { }

    public override NodeStatus Execute()
    {
        return CheckCondition() ?
            NodeStatus.Success :
            NodeStatus.Failure;
    }

    protected abstract bool CheckCondition();
}

/// <summary>
/// 状态切换动作节点
/// </summary>
public class SwitchStateNode : BTNode
{
    private FSM targetFSM;
    private StateType targetState;

    public SwitchStateNode(
        Blackboard bb,
        FSM fsm,
        StateType state
    ) : base(bb)
    {
        targetFSM = fsm;
        targetState = state;
    }

    public override NodeStatus Execute()
    {
        if (!targetFSM.IsCurrentState(targetState))
        {
            targetFSM.SwitchState(targetState);
        }
        return NodeStatus.Success;
    }
}

/// <summary>
/// 行为树执行器
/// </summary>
public class BehaviorTree
{
    private BTNode rootNode;
    private Blackboard blackboard;

    public BehaviorTree(Blackboard bb, BTNode root)
    {
        blackboard = bb;
        rootNode = root;
    }

    public void Tick()
    {
        rootNode?.Execute();
    }
}