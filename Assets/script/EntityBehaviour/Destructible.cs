using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : Entity
{
    public override void ResetEntity()
    {
        base.ResetEntity();
        entityAnimator.SetBool(DestructibleAnimHash.Death, false);
        entityRigidbody.simulated = true;
    }
    protected override void Die()
    {
        base.Die();
        entityAnimator.SetBool(DestructibleAnimHash.Death, true);
        entityRigidbody.simulated = false;
    }
}
public static class DestructibleAnimHash
{
    public static readonly int Death = Animator.StringToHash("Death");
}
