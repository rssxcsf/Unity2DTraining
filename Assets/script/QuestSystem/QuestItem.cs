using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class QuestItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Text UIQuestTitle;
    [SerializeField] private Image UIQuestStatus;
    private LocalData questLocalData;
    private ActiveQuest quest;
    private QuestPanel uiParent;

    private void Awake()
    {

    }
    public void Initialize(ActiveQuest questData, QuestPanel panel)
    {
        quest = questData;
        uiParent = panel;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        UIQuestTitle.text = quest.Data.Name;
        UIQuestStatus.color = GetStatusColor(quest.status);
    }

    public Color GetStatusColor(QuestStatus status) => status switch
    {
        QuestStatus.Completable => Color.yellow,
        QuestStatus.Claimed => Color.green,
        _ => Color.gray
    };
    public void OnItemClicked()
    {
        uiParent.ShowDetail(quest);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        uiParent.ShowDetail(quest);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}