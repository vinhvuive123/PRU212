using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class UIPauseGame : MonoBehaviour
{
    [SerializeField] private GameObject menu ;

    private int score;
    private bool isEnable;

    void Start()
    {
        score = 0;
        isEnable = false;
        menu.SetActive(false);
        ShowGameOver(false);

        endGameView.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isEnable) {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }
    public void ChangeScene(int key)
    {
        if (key==10)
        {
            ResumeGame();
            return;
        }
        UIController.ChangeScene(key);
    }

    public void  SetScore(int key)
    {
        score += key;
    }
    public void PauseGame()
    {
        isEnable = true;
        menu.SetActive(true);
        SetTime(true);
    }

    public void ResumeGame()
    {
        isEnable= false;
        menu.SetActive(false);
        SetTime(false);
    }
    public void SetTime(bool key)
    {
        Time.timeScale = key? 0:1;
    }
    public GameObject endGameView;
    public void ShowGameOver(bool key)
    {
        SetTime(key);
        endGameView.SetActive(true);

    }
    public void ChangeSence()
    {
        SceneManager.LoadScene(3);
    }




    public void Restart()
    {
        DOTween.KillAll();

        score = 0;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        ShowGameOver(false);
    }
}
