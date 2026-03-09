using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class QuestPanel : BasePanel
{
    [Header("UI References")]
    [SerializeField] private QuestItem questItemPrefab;
    [SerializeField] private ScrollRect questListScroll;
    [SerializeField] private Text questNameText;
    [SerializeField] private Text questDescriptionText;
    [SerializeField] private Text questProgressText;
    [SerializeField] private GameObject questListPanel;
    [SerializeField] private GameObject questDetailPanel;
    [SerializeField] private Button actionButton;
    [SerializeField] private Text actionButtonText;

    private Dictionary<string, ActiveQuest> questsDictionary;
    private ActiveQuest currentSelectedQuest;

    protected override void Awake()
    {
        base.Awake();
        InitializeData();
        SetupButtonListeners();
        RefreshQuestList();
        ShowList();
    }

    private void InitializeData()
    {
        List<ActiveQuest> quests = LocalData.Instance.LoadActiveQuestsData();
        questsDictionary = quests.ToDictionary(q => q.Data.Name);
    }

    private void SetupButtonListeners()
    {
        actionButton.onClick.AddListener(OnActionButtonClicked);
    }

    public void ShowList()
    {
        questDetailPanel.SetActive(false);
        questListPanel.SetActive(true);
    }

    public void ShowDetail(ActiveQuest quest)
    {
        currentSelectedQuest = quest;
        UpdateDetailDisplay(quest);
        questListPanel.SetActive(false);
        questDetailPanel.SetActive(true);
    }

    private void RefreshQuestList()
    {
        ClearQuestList();
        PopulateQuestList();
    }

    private void ClearQuestList()
    {
        foreach (Transform child in questListScroll.content)
        {
            Destroy(child.gameObject);
        }
    }

    private void PopulateQuestList()
    {
        foreach (var quest in questsDictionary.Values)
        {
            var questItem = Instantiate(questItemPrefab, questListScroll.content);
            questItem.Initialize(quest, this);
        }
    }

    public void UpdateDetailDisplay(ActiveQuest quest)
    {
        questNameText.text = quest.Data.Name;
        questDescriptionText.text = quest.Data.Description;
        UpdateProgressDisplay(quest);
        UpdateActionButton(quest.status);
    }

    private void UpdateProgressDisplay(ActiveQuest quest)
    {
        questProgressText.text = quest.currentAmount + "/" + quest.Data.RequiredAmount;
    }

    private void UpdateActionButton(QuestStatus status)
    {
        actionButtonText.text = GetStatusText(status);
        actionButton.interactable = status == QuestStatus.Completable;
    }

    private string GetStatusText(QuestStatus status)
    {
        return status switch
        {
            QuestStatus.NotAccepted => "未接取",
            QuestStatus.InProgress => "进行中",
            QuestStatus.Completable => "可完成",
            QuestStatus.Claimed => "已完成",
            _ => "未知状态"
        };
    }

    private void OnActionButtonClicked()
    {
        if (currentSelectedQuest?.status != QuestStatus.Completable) return;

        GrantRewards();
        UpdateQuestStatus();
        RefreshQuestList();
        ShowList();
    }

    private void GrantRewards()
    {
        // 实现具体的奖励发放逻辑
        Debug.Log($"发放奖励：{currentSelectedQuest.Data.Name}");
    }

    private void UpdateQuestStatus()
    {
        currentSelectedQuest.status = QuestStatus.Claimed;
        // 更新本地数据
        LocalData.Instance.Save();
    }

    public void CloseQuestPanel()
    {
        UIManager.Instance.ClosePanel(UIConst.QuestPanel);
    }
}