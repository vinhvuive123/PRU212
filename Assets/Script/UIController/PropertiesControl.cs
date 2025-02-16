using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PRU_project;

public class PropertiesControl : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI P1, P2;

    [SerializeField]
    private Slider Hp1, Hp2;

    [SerializeField]
    private Image color1, color2;

    [SerializeField]
    private GameController gameController;

    int point1 = 0;
    int point2 = 0;

    public List<GameObject> items1, items2;
    public List<Sprite> ItemImageSrc;
    public Sprite dotSprite;

    // Define colors for different health levels
    private Color green = Color.green;
    private Color yellow = Color.yellow;
    private Color orange = new Color(1f, 0.5f, 0f);
    private Color red = Color.red;
    private Color darkRed = new Color(0.5f, 0f, 0f);


    private int maxHealth;

    void Start()
    {
        point1 = 1;
        point2 = 1;
        P1.text = "1";
        P2.text = "1";
        Hp1.value = 1;
        Hp2.value = 1;

        maxHealth = gameController.maxHealth;
        DOTween.SetTweensCapacity(0, 50);

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    IncreaseHP(1, 5);
        //    IncreaseHP(2, 6);
        //}
    }

    public void IncreaseHP(int player, int amount)
    {
        if (player == 1)
        {
            if (point1 > maxHealth) return;
            point1 += amount;
            P1.text = point1.ToString();
            Hp1.DOValue(point1, 0.4f).SetEase(Ease.InCubic);
            UpdateHealthColor(point1, color1);
        }
        else if (player == 2)
        {
            if (point2 > maxHealth) return;
            point2 += amount;
            P2.text = point2.ToString();
            Hp2.value = point2;
            UpdateHealthColor(point2, color2);
        }
    }

    private void UpdateHealthColor(float point, Image healthImage)
    {
        // Map normalizedHp to a percentage from 1 to 100
        float normalizedHp = Mathf.Clamp(point / maxHealth * 100f, 1f, 100f);

        if (normalizedHp > 75f)
        {
            // Transition smoothly from darkRed to red
            healthImage.DOColor(Color.Lerp(darkRed, red, (normalizedHp - 75f) / 25f), 1f);
        }
        else if (normalizedHp > 50f)
        {
            // Transition smoothly from red to orange
            healthImage.DOColor(Color.Lerp(red, orange, (normalizedHp - 50f) / 25f), 1f);
        }
        else if (normalizedHp > 25f)
        {
            // Transition smoothly from orange to yellow
            healthImage.DOColor(Color.Lerp(orange, yellow, (normalizedHp - 25f) / 25f), 1f);
        }
        else if (normalizedHp > 10f)
        {
            healthImage.DOColor(Color.Lerp(yellow, green, normalizedHp / 25f), 1f);
        }
        else
        {
            // Transition from yellow to green
            //healthImage.DOColor(Color.Lerp(yellow, green, normalizedHp / 25f), 1f);
        }
    }
    public void ResetProp(int player)
    {
        if (player == 1)
        {
            point1 = 1;
            P1.text = point1.ToString();
            Hp1.value = 0;
            color1.color = green;
        }
        else if (player == 2)
        {
            point2 = 1;
            P2.text = point2.ToString();
            Hp2.value = 0;
            color2.color = green;
        }
    }

    public void ChangeItemView(int player, ItemType type, int itemNumber)
    {
        List<GameObject> selectedItems = player == 1 ? items1 : items2;

        if (itemNumber < 0 || itemNumber >= selectedItems.Count)
        {
            Debug.LogWarning("Item number is out of range");
            return;
        }

        int typeIndex = (int)type;
        if (typeIndex < 0 || typeIndex >= ItemImageSrc.Count)
        {
            Debug.LogWarning("Invalid item type");
            return;
        }

        selectedItems[itemNumber].GetComponent<Image>().sprite = ItemImageSrc[typeIndex];
        selectedItems[itemNumber].GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
    }


    public void ThrowItem(int player, int itemIndex, List<ItemType> inventory)
    {
        List<GameObject> selectedItems = player == 1 ? items1 : items2;
        selectedItems[itemIndex].GetComponent<Image>().sprite = dotSprite;
        selectedItems[itemIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);

        // Reorder or update the remaining inventory items on the UI
        UpdateInventoryUI(player, inventory);
    }
    private void UpdateInventoryUI(int player, List<ItemType> inventory)
    {
        List<GameObject> selectedItems = player == 1 ? items1 : items2;

        // Update each slot in the UI based on current inventory
        for (int i = 0; i < selectedItems.Count; i++)
        {
            if (i < inventory.Count)
            {
                ItemType itemType = inventory[i];
                int typeIndex = (int)itemType;
                selectedItems[i].GetComponent<Image>().sprite = ItemImageSrc[typeIndex];
                selectedItems[i].GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
            }
            else
            {
                // Clear empty slots
                selectedItems[i].GetComponent<Image>().sprite = dotSprite;
                selectedItems[i].GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
            }
        }
    }
}

