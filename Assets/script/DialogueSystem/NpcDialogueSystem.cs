using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NpcDialogueSystem : MonoBehaviour
{
    [SerializeField]
    [Tooltip("拖放对话数据到这里")]
    private Dialogue dialogue;//对话数据
    private BasePanel dialogBox;//对话框预制体
    private Text dialog;//对话内容文本UI
    private Text characterName;//对话人名字UI
    [SerializeField] private CharacterType type;//对话NPC类型，暂时没啥用
    private int index;
    private bool isTyping;
    private bool isPlayEnd;
    private void Start()
    {
        dialogBox = UIManager.Instance.GetPanel("DialoguePanel");
        GetTextTransform();
    }
    private void Update()
    {
        
    }
    public void Communicate()
    {
        isPlayEnd = false;
        if (isTyping)
        {
            // 跳过当前打字
            StopAllCoroutines();
            dialog.text = dialogue.dialogNodes[index - 1].dialog;
            isTyping = false;
        }
        else
            Play();
    }
    private void Play()
    {
        if (index >= dialogue.dialogNodes.Length)
        {
            EndDialogue();
            return;
        }
        dialogBox.SetActive(true);
        if (isTyping)
            return;
        else
        {
            isTyping = true;
            DialogNode node = dialogue.dialogNodes[Mathf.Clamp(index++, 0, dialogue.dialogNodes.Length - 1)];
            characterName.text = node.name;
            StartCoroutine(TypeText(node.dialog));
        }
    }
    private void GetTextTransform()
    {
        if (dialogBox == null)
            dialogBox = UIManager.Instance.OpenPanel("DialoguePanel");
        dialogBox.SetActive(false);
        Transform nameTransform = dialogBox.transform.Find("Name");
        Transform contentTransform = dialogBox.transform.Find("content");
        if (nameTransform != null)
        {
            characterName = nameTransform.GetComponentInChildren<Text>();
            if (characterName == null)
            {
                Debug.LogError("Name下未找到Text组件");
            }
        }
        if (contentTransform != null)
        {
            dialog = contentTransform.GetComponentInChildren<Text>();
            if (characterName == null)
            {
                Debug.LogError("content下未找到Text组件");
            }
        }
    }
    IEnumerator TypeText(string text)
    {
        dialog.text = "";
        foreach (char c in text)
        {
            dialog.text += c;
            yield return new WaitForSeconds(0.1f); // 打字机效果速度
        }
        isTyping = false;
    }
    private void EndDialogue()
    {
        dialogBox.SetActive(false);
        isPlayEnd = true;
        index = 0; // 重置索引
    }
    public bool returnDialogBoxState()
    {
        return isPlayEnd;
    }
}
