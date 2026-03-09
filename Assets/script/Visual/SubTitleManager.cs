using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance; // 单例模式

    [Header("UI References")]
    [SerializeField] private GameObject SubtitlePanelPrefab;

    private GameObject subtitlePanel;
    private Text subtitleText;
    private Text TipText;

    [Header("Settings")]
    [SerializeField] private float charactersPerSecond = 30f; // 打字速度
    [SerializeField] private float lineDuration = 3f;        // 单行显示时间
    [SerializeField] private float lineInterval = 0.5f;      // 行间间隔

    private Queue<string> subtitleQueue = new Queue<string>();
    private bool isProcessing = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 每次场景加载后重新初始化UI组件
        CreateSubtitlePanel();
    }
    void CreateSubtitlePanel()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (SubtitlePanelPrefab != null && canvas != null)
        subtitlePanel = Instantiate(SubtitlePanelPrefab, canvas.transform);
        subtitleText = subtitlePanel.transform.Find("Bottom/SubTitleText").GetComponent<Text>();
        TipText = subtitlePanel.transform.Find("Top/TipText").GetComponent<Text>();
    }

    public void ShowSubtitles(params string[] lines)
    {
        foreach (var line in lines) subtitleQueue.Enqueue(line);
        if (!isProcessing) StartCoroutine(ProcessQueue());
    }
    public void ShowTips(string str)
    {
        StopCoroutine(nameof(GenerateTips));
        StartCoroutine(GenerateTips(str));
    }
    private IEnumerator GenerateTips(string str)
    {
        TipText.text = str;
        yield return new WaitForSeconds(2f);
        TipText.text = "";
    }

    private IEnumerator ProcessQueue()
    {
        isProcessing = true;
        while (subtitleQueue.Count > 0)
        {
            string currentLine = subtitleQueue.Dequeue();
            yield return StartCoroutine(TypeText(currentLine));
            yield return new WaitForSeconds(lineDuration);
            yield return StartCoroutine(ClearText());
            yield return new WaitForSeconds(lineInterval);
        }
        isProcessing = false;
    }

    private IEnumerator TypeText(string line)
    {
        subtitleText.text = "";
        float delay = 1 / charactersPerSecond;

        foreach (char c in line)
        {
            subtitleText.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
    private IEnumerator ClearText()
    {
        string original = subtitleText.text;
        for (int i = original.Length; i >= 0; i--)
        {
            subtitleText.text = original.Substring(0, i);
            yield return new WaitForSeconds(0.02f);
        }
    }
}
