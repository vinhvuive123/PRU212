using Assets.Script.Models;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMoveController controls;

    private bool isControllerPlayer;

    public void Initialize(PlayerMoveController controls, bool isControllerPlayer)
    {
        this.controls = controls;
        this.isControllerPlayer = isControllerPlayer;

        //Debug.Log(gameObject.name + " isController : " + isControllerPlayer);
        if (isControllerPlayer)
        {
            controls.Controller.LightAttack.performed += ctx => Attack();
        }
        else
        {
            controls.Keyboard.LightAttack.performed += ctx => Attack();
        }

    }


    [SerializeField]
    private GameObject Animation;
    [Header("Force")]
    //[SerializeField] float forceNlight = 10;
    //[SerializeField] private float knockbackForce = 50f;
    [SerializeField] private List<AttackProperties> listAttack;


    private BoxCollider2D attackTriggerCollider;
    private BoxCollider2D playerCollider;
    private void Start()
    {
        attackTriggerCollider = Animation.GetComponent<BoxCollider2D>();
        attackTriggerCollider.isTrigger = true;
        attackTriggerCollider.enabled = false;

        playerCollider = gameObject.GetComponent<BoxCollider2D>();
        // config attack state : sideLight, neturalLigth
        listAttack = new()
        {
            new AttackProperties {AnimationName = "Player_nlight", StartAnimation= 0.2f, EndAnimation =0.5f, ForceX=1.7f},
            new AttackProperties {AnimationName = "Player_slight", StartAnimation= 0.1f, EndAnimation =0.5f, ForceX=1.6f},
            new AttackProperties {AnimationName = "Player_dlight", StartAnimation= 0.1f, EndAnimation =0.5f, ForceX=1.6f},
            new AttackProperties {AnimationName = "Player_nair", StartAnimation= 0.1f, EndAnimation =0.5f, ForceX=1.6f},
            new AttackProperties {AnimationName = "Player_sair", StartAnimation= 0.1f, EndAnimation =0.5f, ForceX=1.6f},
            new AttackProperties {AnimationName = "Player_dair", StartAnimation= 0.1f, EndAnimation =0.5f, ForceX=1.6f},
        };
    }
    void Update()
    {
        HitBoxAppearConfig();

    }
    private void HitBoxAppearConfig()
    {
        var animationState = Animation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        float attackTime = Animation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
        foreach (var attackProp in listAttack)
        {
            if (animationState.IsName(attackProp.AnimationName))
            {
                if (attackTime >= attackProp.StartAnimation && attackTime <= attackProp.EndAnimation)
                    attackTriggerCollider.enabled = true;
                else
                {
                    attackTriggerCollider.enabled = false;
                }
            }
        }
    }

    private void Attack()
    {
        float speed = gameObject.GetComponent<PlayerMove>().GetSpeed();

        // ligth attack
        if (gameObject.GetComponent<PlayerMove>().IsGrounded())
        {
            bool checkDowKey = isControllerPlayer ? controls.Controller.DownMovement.ReadValue<float>() > 0 : controls.Keyboard.DownMovement.ReadValue<float>() > 0;

            // downd ligth
            if (checkDowKey) { Dlight(1.7f, "dlight"); return; }
            // downd light
            if (speed == 0) { Nlight(1.7f, "nlight"); return; }
            // sigth light
            else { Slight(1.7f, "slight"); return; }
        }
        // air attack 
        else
        {
            bool checkDowKey = isControllerPlayer ? controls.Controller.DownMovement.ReadValue<float>() > 0 : controls.Keyboard.DownMovement.ReadValue<float>() > 0;

            // downd air
            if (checkDowKey) { Dair(1.7f, "dair"); return; }
            // downd air
            if (speed == 0) { Nair(1.7f, "nair"); return; }
            // sigth air
            else { Sair(1.7f, "sair"); return; }
        }
    }


    private void Nair(float force, string trigger)
    {
        Animation.GetComponent<Animator>().SetTrigger(trigger);
    }

    private void Sair(float force, string trigger)
    {
        float speed = gameObject.GetComponent<PlayerMove>().GetSpeed();

        Animation.GetComponent<Animator>().SetTrigger(trigger);
    }

    public Ease testMoveX;
    private void Dair(float force, string trigger)
    {
        float speed = gameObject.GetComponent<PlayerMove>().GetSpeed();

        Animation.GetComponent<Animator>().SetTrigger(trigger);
        float direction = Animation.transform.rotation.y == 0 ? 1 : -1;
        transform.DOMoveX(transform.position.x + force * direction, 0.3f).SetEase(Ease.InSine).OnComplete(() =>
        {
            gameObject.GetComponent<PlayerMove>().SetSpeed(0);
        });

    }

    private void Nlight(float force, string trigger)
    {
        Animation.GetComponent<Animator>().SetTrigger(trigger);
        //float direction = Animation.transform.rotation.y == 0 ? 1 : -1;
        //transform.DOMoveX(transform.position.x + force * direction, 0.7f).SetEase(Ease.OutQuint);
    }

    private void Slight(float force, string trigger)
    {
        float speed = gameObject.GetComponent<PlayerMove>().GetSpeed();

        Animation.GetComponent<Animator>().SetTrigger(trigger);
        float direction = Animation.transform.rotation.y == 0 ? 1 : -1;
        transform.DOMoveX(transform.position.x + force * direction, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameObject.GetComponent<PlayerMove>().SetSpeed(0);
        });
    }
    private void Dlight(float force, string trigger)
    {
        float speed = gameObject.GetComponent<PlayerMove>().GetSpeed();

        Animation.GetComponent<Animator>().SetTrigger(trigger);
        float direction = Animation.transform.rotation.y == 0 ? 1 : -1;
        transform.DOMoveX(transform.position.x + force * direction, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameObject.GetComponent<PlayerMove>().SetSpeed(0);
        });
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("hit enemy : " + other.name);

            //Enemy enemy = other.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    Debug.Log($"Attack hit enemy: {enemy.enemyType}");
            //    Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();

            //    enemy.TakeDamage(50);
            //    // direction 
            //    // normalized make vertor 1
            //    Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
            //    enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            //}
        }
    }

    private void OnDestroy()
    {
        //controls.ActionMap.LightAttack.performed -= ctx => Attack();
    }
}
