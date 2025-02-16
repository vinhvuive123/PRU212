using PRU_project;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProp : MonoBehaviour
{
    public GameObject fatherItem;
    [SerializeField] private ItemType type;
    
    public float strength = 2;

    public ItemType Type { get => type; set => type = value; }

}
