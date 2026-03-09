using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [Header("»ù´¡ÉèÖÃ")]
    [SerializeField] protected string skillName;
    [SerializeField, TextArea] protected string description;
    [SerializeField] protected Sprite icon;

    [Header("ÀäÈ´ÉèÖÃ")]
    [SerializeField] protected float cooldown = 5f;
    [SerializeField] protected float currentCooldown = 0f;

    [SerializeField] protected GameObject summonObject;

    [SerializeField] protected int manaCost;
    [SerializeField] protected int skillDamage;
    public abstract void Launch(Transform transform);
    public Sprite Icon => icon;
    public float ReturnSkillCoolDownTime => cooldown;
    public int ReturnManaCost => manaCost;
}
