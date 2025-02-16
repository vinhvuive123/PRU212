using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownManager  : MonoBehaviour
{

    public TextMeshProUGUI countdownText; 
    public int countdownTime = 3;
    public GameController gameController;
    public GameObject vitualCamera;
    void Start()
    {
        vitualCamera.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            countdownText.text = countdownTime.ToString();
            yield return new WaitForSeconds(1f); 
            countdownTime--;
        }

        // Countdown finished
        countdownText.text = "Start!";

        yield return new WaitForSeconds(1f); 

        countdownText.gameObject.SetActive(false); 
        StartGame(); 
    }

    private void StartGame()
    {
        gameController.SetBeginPosition();
        vitualCamera.SetActive(true);
    }
}
