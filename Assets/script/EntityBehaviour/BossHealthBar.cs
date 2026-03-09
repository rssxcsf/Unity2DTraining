using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : BasePanel
{
    public static int FadeHealth;
    public static int CurrentHealth;
    public static int MaxHealth;
    public Image health;
    public Image fadeHealth;
    public Image Icon;
    void Start()
    {
        CurrentHealth = MaxHealth;
        FadeHealth = MaxHealth;
        LoadBossIcon();
    }
    void LoadBossIcon()
    {
        Texture2D t = (Texture2D)Resources.Load(BossConest.QueenIcon);
        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
        Icon.GetComponent<Image>().sprite = temp;
    }
    void Update()
    {
        SwitchCombatBgm();
        UpdateHealthBar();
    }
    public static void UpdateHealth(int health)
    {
        CurrentHealth = Mathf.Clamp(health, 0, MaxHealth);
    }
    private void UpdateHealthBar()
    {
        health.fillAmount = CalculateProportion();
        fadeHealth.fillAmount = Mathf.Lerp(fadeHealth.fillAmount, health.fillAmount, 10f * Time.deltaTime);
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }
    private float CalculateProportion()
    {
        return (float)CurrentHealth / (float)MaxHealth;
    }
    private void SwitchCombatBgm()
    {
        if (CalculateProportion() > 0.7)
            AudioPlayer.Instance.SetPhase(0);
        if (CalculateProportion() <= 0.7 && CalculateProportion() > 0.4)
            AudioPlayer.Instance.SetPhase(1);
        if(CalculateProportion() <= 0.4 && CalculateProportion() > 0.1)
            AudioPlayer.Instance.SetPhase(2);
        if(CalculateProportion() <= 0.1)
            AudioPlayer.Instance.SetPhase(3);
    }
}

public class BossConest
{
    public const string QueenIcon = "texture/GUI/healthbar/Icon/queen";
}
