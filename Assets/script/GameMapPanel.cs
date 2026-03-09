using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMapPanel : BasePanel
{
    public void LoadPlain()
    {
        SceneManager.LoadScene(3);
        Time.timeScale = 1.0f;
    }
    public void LoadForest()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1.0f;
    }
    public void LoadEmpire()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1.0f;
    }
    public void LoadField()
    {
        SceneManager.LoadScene(4);
        Time.timeScale = 1.0f;
    }
}
