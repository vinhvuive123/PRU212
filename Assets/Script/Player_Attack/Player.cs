using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int life = 3;
    public float vulnerabilityIndex = 1;

    [SerializeField] private GameObject uiControl;
    [SerializeField] private PropertiesControl propControl;


    public float GetVulnerability() => vulnerabilityIndex;
    public void TakeDamage(float damage, int type)
    {
        vulnerabilityIndex += damage;
             propControl.IncreaseHP(type, (int) damage);
    }
}
