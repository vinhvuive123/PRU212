using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    void Start()
    {
        
        
        
    }
  
    public static void  ChangeScene(int index)
    {
       switch (index)
        {
            case 0:
                SceneManager.LoadScene(1);
                break;
            case 1:
                SceneManager.LoadScene("MenuScene");
                break;
            case 2:
                break;
        }
    }
    
}
