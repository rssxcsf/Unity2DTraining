using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager instance;
    private Transform _uiRoot;
    //路径配置字典
    private Dictionary<string, string> pathDict;
    //预制件缓存字典
    private Dictionary<string, GameObject> prefabDict;
    //已打开界面的缓存字典
    public Dictionary<string,BasePanel> panelDict;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UIManager();
            }
            return instance;
        }
    }
    public Transform UIRoot
    {
        get
        {
            if(_uiRoot == null)
            {
                if(GameObject.Find("Canvas"))
                {
                    _uiRoot = GameObject.Find("Canvas").transform;
                }
                else
                {
                    Debug.Log("未创建UI面板");
                }
            };
            return _uiRoot;
        }
    }
    private UIManager()
    {
        InitDicts();
    }
    private void InitDicts()
    {
        prefabDict = new Dictionary<string,GameObject>();
        panelDict = new Dictionary<string, BasePanel>();
        pathDict = new Dictionary<string, string>()
        {
            {UIConst.QuestPanel,"Quest/QuestPanel" },
            {UIConst.DialoguePanel,"DialoguePanel" },
            {UIConst.InventoryPanel,"Inventory/InventoryPanel" },
            {UIConst.PlayerHealthBarPanel,"StateBar/PlayerStateBarPanel" },
            {UIConst.BossHealthBarPanel,"StateBar/BossHealthBarPanel" },
            {UIConst.PauseMenu,"Menu/PauseMenu" },
            {UIConst.GameOverPanel,"Menu/GameOverPanel" },
            {UIConst.GameMapPanel,"GameMap/GameMapPanel" }
        };
    }
    public BasePanel GetPanel(string name)
    {
        BasePanel panel = null;
        //检查是否已打开
        if (panelDict.TryGetValue(name,out panel))
        {
            return panel;
        }
        return null;
    }
    public BasePanel OpenPanel(string name)
    {
        BasePanel panel = null;
        if(panelDict.TryGetValue(name, out panel))
        {
            Debug.Log("界面已打开" + name);
            return null;
        }
        string path = "";
        if(!pathDict.TryGetValue(name, out path))
        {
            Debug.Log("界面名称错误，或未配置路径" + name);
            return null;
        }
        //使用缓存的预制件
        GameObject panelPrefab = null;
        if(!prefabDict.TryGetValue(name, out panelPrefab))
        {
            string realPath = "Prefab/Panel/" + path;
            panelPrefab = Resources.Load(realPath) as GameObject;
            prefabDict.Add(name, panelPrefab);
        }
        GameObject panelObject = GameObject.Instantiate(panelPrefab,UIRoot,false);
        panel = panelObject.GetComponent<BasePanel>();
        panelDict.Add(name, panel);
        panel.OpenPanel(name);
        return panel;
    }
    public bool ClosePanel(string name)
    {
        BasePanel panel = null;
        if(!panelDict.TryGetValue(name,out panel))
        {
            Debug.Log("界面未打开:" + name);
            return false;
        }
        panel.ClosePanel();
        return true;
    }
}

public class BasePanel : MonoBehaviour
{
    protected bool isRemove = false;
    protected string PanelName;
    protected virtual void Awake()
    {
        
    }
    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    public virtual void OpenPanel(string name)
    {
        PanelName = name;
    }
    public virtual void ClosePanel()
    {
        if (!string.IsNullOrEmpty(PanelName) && UIManager.Instance.panelDict.ContainsKey(PanelName))
        {
            UIManager.Instance.panelDict.Remove(PanelName);
            Debug.Log($"【主动关闭】{PanelName} 面板已移除");
        }
        if (gameObject != null)
            Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (PanelName != null)
        {
            UIManager.Instance.panelDict.Remove(PanelName);
            Debug.Log($"【自动清理】{PanelName} 面板已从字典移除");
        }
        else
        {
            Debug.Log("Panel名为空");
        }
    }
}
public class UIConst
{
    public const string QuestPanel = "QuestPanel";
    public const string DialoguePanel = "DialoguePanel";
    public const string InventoryPanel = "InventoryPanel";
    public const string PlayerHealthBarPanel = "PlayerHealthBarPanel";
    public const string BossHealthBarPanel = "BossHealthBarPanel";
    public const string PauseMenu = "PauseMenu";
    public const string GameOverPanel = "GameOverPanel";
    public const string GameMapPanel = "GameMapPanel";
}
