using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StatePanel : BasePanel
{
    public static int FadeHealth;
    public static int CurrentHealth;
    public static int MaxHealth;
    public static int CurrentMana;
    public static int FadeMana;
    public static int MaxMana;

    public Image health;
    public Image fadeHealth;

    public Image mana;
    public Image fadeMana;

    public List<Image> CoolDown;

    [SerializeField] private Transform UIPotion;

    private List<InventoryItem> PotionList;

    private float DrinkCoolDownTimerAlpha1;
    private float DrinkCoolDownTimerAlpha2;
    [SerializeField] private float DrinkCoolDownTimeAlpha1;
    [SerializeField] private float DrinkCoolDownTimeAlpha2;

    private bool Alpha1Input;
    private bool Alpha2Input;
    protected override void Awake()
    {
        PotionList = InventoryManager.Instance.GetPotionsLocalData();
        DrinkCoolDownTimerAlpha1 = 0;
        DrinkCoolDownTimerAlpha2 = 0;

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("【状态栏】InventoryManager实例不存在!");
        }
    }
    void Start()
    {
        CurrentHealth = MaxHealth;
        FadeHealth = MaxHealth;

        CurrentMana = MaxMana;
        FadeMana = MaxMana;
    }
    void Update()
    {
        UpdateInput();
        UpdateGUI();
        UpdatePotion();
        DrinkPotion();
    }
    private void UpdateInput()
    {
        Alpha1Input = Input.GetKeyDown(KeyCode.Alpha1);
        Alpha2Input = Input.GetKeyDown(KeyCode.Alpha2);
    }
    private void UpdateGUI()
    {
        UpdateHealthBar();
        UpdatManaBar();
        UpdateCoolDown();
    }

    private void UpdateCoolDown()
    {
        if (DrinkCoolDownTimerAlpha1 < DrinkCoolDownTimeAlpha1)
            DrinkCoolDownTimerAlpha1 += Time.deltaTime;
        if (DrinkCoolDownTimerAlpha2 < DrinkCoolDownTimeAlpha2)
            DrinkCoolDownTimerAlpha2 += Time.deltaTime;
        CoolDown[0].fillAmount = 1-(DrinkCoolDownTimerAlpha1 / DrinkCoolDownTimeAlpha1);
        CoolDown[1].fillAmount = 1 - (DrinkCoolDownTimerAlpha2 / DrinkCoolDownTimeAlpha2);
    }
    private void UpdatePotion()
    {
        foreach (InventoryItem item in PotionList)
        {
            if (item.type == ItemType.Potion)
            {
                if (item.id == 1)
                {
                    if (item.stackCount != 0)
                        UIPotion.Find("HealthPotion/Amount").GetComponent<Text>().text = "X" + item.stackCount.ToString();
                    UIPotion.Find("HealthPotion").gameObject.SetActive(true);
                }
                if (item.id == 2)
                {
                    if (item.stackCount != 0)
                        UIPotion.Find("ManaPotion/Amount").GetComponentInChildren<Text>().text = "X" + item.stackCount.ToString();
                    UIPotion.Find("ManaPotion").gameObject.SetActive(true);
                }
            }
        }
    }
    void UpdateHealthBar()
    {
        health.fillAmount = (float)CurrentHealth/(float)MaxHealth;
        fadeHealth.fillAmount = Mathf.Lerp(fadeHealth.fillAmount, health.fillAmount, 10f*Time.deltaTime);
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }
    void UpdatManaBar()
    {
        mana.fillAmount = (float)CurrentMana / (float)MaxMana;
        fadeMana.fillAmount = Mathf.Lerp(fadeMana.fillAmount, mana.fillAmount, 10f * Time.deltaTime);
        CurrentMana = Mathf.Clamp(CurrentMana, 0, MaxMana);
    }
    public static void UpdateHealth(int health)
    {
        CurrentHealth = Mathf.Clamp(health,0,MaxHealth);
    }
    public static void UpdateMana(int mana)
    {
        CurrentMana = Mathf.Clamp(mana, 0, MaxMana);
    }
    public void DrinkPotion()
    {
        if (Alpha1Input)
        {
            DrinkHealthPotion();
        }
        else if(Alpha2Input)
        {
            DrinkManaPotion();
        }
    }
    private void DrinkHealthPotion()
    {
        foreach (InventoryItem item in PotionList)
        {
            if (item.type == ItemType.Potion && item.id == 1)
            {
                if (item.stackCount >= 0)
                {
                    if (DrinkCoolDownTimerAlpha1 >= DrinkCoolDownTimeAlpha1)
                    {
                        if (CurrentHealth != MaxHealth)
                        {
                            item.stackCount--;
                            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Heal(20);
                            DrinkCoolDownTimerAlpha1 = 0;
                        }
                        else
                            SubtitleManager.Instance.ShowTips("当前血量已满");
                    }
                    else
                        SubtitleManager.Instance.ShowTips("药水冷却中");
                }
                break;
            }
            else
                SubtitleManager.Instance.ShowTips("没有该药水");
        }
    }
    private void DrinkManaPotion()
    {
        foreach (InventoryItem item in PotionList)
        {
            if (item.type == ItemType.Potion && item.id == 2)
            {
                if (item.stackCount >= 0)
                {
                    if (DrinkCoolDownTimerAlpha2 >= DrinkCoolDownTimeAlpha2)
                    {
                        if (CurrentMana != MaxMana)
                        {
                            item.stackCount--;
                            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().RestoreMana(10);
                            DrinkCoolDownTimerAlpha2 = 0;
                        }
                        else
                            SubtitleManager.Instance.ShowTips("当前法力值已满");
                    }
                    else
                        SubtitleManager.Instance.ShowTips("药水冷却中");
                }
                break;
            }
            else
                SubtitleManager.Instance.ShowTips("没有该药水");
        }
    }
}
