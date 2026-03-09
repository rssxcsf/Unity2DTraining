using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] private Skill SkillA;
    [SerializeField] private Skill SkillB;
    [SerializeField] private Skill SkillC;
    private float SkillACoolDownTimer;
    private float SkillBCoolDownTimer;
    private float SkillCCoolDownTimer;
    private Transform SkillerTransform;
    private Entity SkillerEntity;
    private Animator SkillerAnimator;
    private void Awake()
    {
        SkillerTransform = GetComponent<Transform>();
        SkillerEntity = GetComponent<Entity>();
        SkillerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if(SkillA != null)
        SkillACoolDownTick();
        if (SkillB != null)
            SkillBCoolDownTick();
        if (SkillC != null)
            SkillCCoolDownTick();
    }
    public void LaunchSkillA()
    {
        if (SkillerEntity.ManaCost(SkillA.ReturnManaCost))
        {
            SkillA.Launch(SkillerTransform);
            SkillerAnimator.SetTrigger("Skill");
            SkillACoolDownTimer = 0;
        }
        else
            SubtitleManager.Instance.ShowTips("没有足够的法力值");
    }
    public void LaunchSkillB()
    {
        if (SkillerEntity.ManaCost(SkillB.ReturnManaCost))
        {
            SkillB.Launch(SkillerTransform);
            SkillerAnimator.SetTrigger("Skill");
            SkillACoolDownTimer = 0;
        }
        else
            SubtitleManager.Instance.ShowTips("没有足够的法力值");
    }
    public void LaunchSkillC()
    {
        if (SkillerEntity.ManaCost(SkillC.ReturnManaCost))
        {
            SkillC.Launch(SkillerTransform);
            SkillerAnimator.SetTrigger("Skill");
            SkillACoolDownTimer = 0;
        }
        else
            SubtitleManager.Instance.ShowTips("没有足够的法力值");
    }
    void SkillACoolDownTick()
    {
        if (SkillACoolDownTimer <SkillA.ReturnSkillCoolDownTime)
            SkillACoolDownTimer += Time.deltaTime;
    }
    public bool IsSkillAReady()
    {
        return SkillACoolDownTimer >= SkillA.ReturnSkillCoolDownTime;
    }
    void SkillBCoolDownTick()
    {
        if (SkillBCoolDownTimer < SkillB.ReturnSkillCoolDownTime)
            SkillBCoolDownTimer += Time.deltaTime;
    }
    public bool IsSkillBReady()
    {
        return SkillBCoolDownTimer >= SkillB.ReturnSkillCoolDownTime;
    }
    void SkillCCoolDownTick()
    {
        if (SkillCCoolDownTimer < SkillC.ReturnSkillCoolDownTime)
            SkillCCoolDownTimer += Time.deltaTime;
    }
    public bool IsSkillCReady()
    {
        return SkillCCoolDownTimer >= SkillC.ReturnSkillCoolDownTime;
    }
}
