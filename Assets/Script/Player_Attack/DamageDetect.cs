using DG.Tweening;
using PRU_project;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetech : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Animator playerAnimator;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ItemThrow"))
        {
            //Debug.Log(player.name + " enter : " + collision.gameObject.name);
            float itemStrenght = collision.gameObject.GetComponent<ItemProp>().strength;
            player.GetComponent<PlayerMove>().ApplyKnockback(collision.transform.position, itemStrenght);
            Destroy(collision.GetComponent<ItemProp>().fatherItem);
        }
    }

}
