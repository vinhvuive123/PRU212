using DG.Tweening;
using PRU_project;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{

    // trigger circle collider for player
    // trigger circle collider for  items
    // if 2 trigger 

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject itemToPickUp;
    [SerializeField] private ItemSpawnPoint itemSpamPoint;
    [SerializeField] private GameObject animationObject;


    private ItemProp itemProps;

    // max inventory = 3 
    public List<ItemType> inventory = new List<ItemType>();
    [SerializeField] private int maxInventory = 3;


    private PlayerMoveController controls;

    private bool isControllerPlayer;

    public void Initialize(PlayerMoveController controls, bool isControllerPlayer)
    {
        this.controls = controls;
        this.isControllerPlayer = isControllerPlayer;

        //Debug.Log(gameObject.name + " isController : " + isControllerPlayer);
        if (isControllerPlayer)
        {
            controls.Controller.Pick.performed += ctx => PickUpItemF();
            controls.Controller.Throw.performed += ctx => ThrowItem();
        }

        else
        {
            controls.Keyboard.Pick.performed += ctx => PickUpItemF();
            controls.Keyboard.Throw.performed += ctx => ThrowItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RangerItem"))
        {
            itemToPickUp = collision.gameObject;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("RangerItem"))
        {
            itemToPickUp = null;
        }
    }
    


    private void Update()
    {

        if (itemToPickUp != null && Input.GetKeyDown(KeyCode.E))
        {

            PickUpItemF();
        }
    }

    [SerializeField]
    private PropertiesControl uiControl;

    private void PickUpItemF()
    {
        //Debug.Log("pick adsnf");
        if (itemToPickUp != null && inventory.Count < maxInventory)
        {
            itemProps = itemToPickUp.GetComponent<ItemProp>();

            inventory.Add(itemProps.Type);
            uiControl.ChangeItemView(isControllerPlayer ? 2 : 1, itemProps.Type, inventory.Count - 1);

            Destroy(itemProps.fatherItem);

            //Debug.Log("Picked up item: " + itemProps.fatherItem.name);

            itemToPickUp = null;

        }
    }

    public void ThrowItem()
    {
        int playerId = isControllerPlayer ? 2 : 1;

        int itemIndex = 0; // Always remove the first item in FIFO

        if (inventory.Count == 0)
        {
            Debug.LogWarning("No items to throw.");
            return;
        }

        // get item type, remove in inventory
        ItemType itemType = inventory[itemIndex];
        inventory.RemoveAt(itemIndex);

        // update ui remove 
        uiControl.ThrowItem(playerId, itemIndex, inventory);

        GameObject itemPrefab = GetByType(itemType);

        float speed = player.GetComponent<PlayerMove>().GetSpeed();
        int direction = player.GetComponent<PlayerMove>().directionB ? 1 : -1;
        Vector3 throwDirection = new Vector3(direction, 0, 0);

        animationObject.GetComponent<Animator>().SetTrigger("slight");

        // throw out item 
        GameObject thrownItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        thrownItem.AddComponent<ItemProp>().Type = itemType;
        //thrownItem.tag = "ItemThrow";

        Rigidbody2D rb2d = thrownItem.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.gravityScale = 0.5f;
            rb2d.AddForce(throwDirection * (speed + 2), ForceMode2D.Impulse);
        }

        // enable hitbox
        Transform hitbox = thrownItem.transform.Find("HitBox");
        if (hitbox != null)
        {
            Debug.Log("find hitbox");
            StartCoroutine(ActivateHitbox(hitbox));
        }
     
        Debug.Log("Thrown item: " + itemType);
    }
    private IEnumerator ActivateHitbox(Transform hitbox)
    {
        yield return new WaitForSeconds(0.3f);
        hitbox.gameObject.SetActive(true);
    }
    private GameObject GetByType(ItemType itemType)
    {
        GameObject itemPrefab = itemSpamPoint.itemPrefabs[0];

        if (itemType == ItemType.Boms)
        {
            itemPrefab = itemSpamPoint.itemPrefabs[0];

        }
        else if (itemType == ItemType.Spike)
        {
            itemPrefab = itemSpamPoint.itemPrefabs[1];

        }
        else if (itemType == ItemType.Sticky)
        {
            itemPrefab = itemSpamPoint.itemPrefabs[2];
        }
        return itemPrefab;
    }
}
