using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu:BasePanel 
{
    protected override void Awake()
    {
        PanelName = "MainMenu";
    }
    public void OpenShop()
    {
        SceneManager.LoadScene("Shop");
        Time.timeScale = 1.0f;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Resume()
    {
        Time.timeScale =1;
        GameManager.Instance.PauseGame();
    }
    public void BackToTitle()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
}
