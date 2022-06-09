using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager Instance;
    int currentScene;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    public void NextScene()
    {
        int nextScene = currentScene < SceneManager.sceneCountInBuildSettings - 1 ? currentScene + 1 : 0;
        SceneManager.LoadScene(nextScene);
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitApp();
    }

    public void OnAgentDied()
    {
        Invoke(nameof(NextScene), 3f);
    }
}
