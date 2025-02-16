using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            KnockOutPlayer(other.gameObject);
        }
        else if (other.CompareTag("Item"))
        {
            Destroy(other.gameObject);
            itemSpamPoint.GetComponent<ItemSpawnPoint>().DecreaseItemCount();
        }
    }


    public void KnockOutPlayer(GameObject player)
    {
        bool isPlayer1 = player.name.Equals("Player1");
        player.SetActive(false);
        Player playerComponent = player.GetComponent<Player>();
        Debug.Log(playerComponent.GetComponent<Player>().life);

        if (playerComponent.life > 0)
        {
            playerComponent.life -= 1;
            if (isPlayer1)
            {
                lifeText1.text = playerComponent.life.ToString();
            }
            else
            {
                lifeText2.text = playerComponent.life.ToString();
            }
        }

        if (playerComponent.life == 0)
        {
            //gameObject.SetActive(false);

            UIController.GetComponent<UIPauseGame>().ShowGameOver(true);

        }


        else if (gameObject.activeInHierarchy)
        {
            StartCoroutine(AutoReload(player, isPlayer1 ? 0 : 1));
        }
    }

    // Deadzone properties
    [Header("Deadzone properties")]

    [SerializeField] private TextMeshProUGUI text1;
    [SerializeField] private TextMeshProUGUI text2;
    public GameObject spamPoint1, spamPoint2;
    [SerializeField] private GameObject animation1, animation2;
    [SerializeField] private float wattingTime = 5f;

    [Header("Item tracking")]
    [SerializeField] private GameObject itemSpamPoint;


    [Header("life manager")]
    [SerializeField] private GameController controller;
    [SerializeField] private GameObject UIController;
    [SerializeField] private TextMeshProUGUI lifeText1, lifeText2;

    [SerializeField] private PropertiesControl propcontrol;

    private IEnumerator AutoReload(GameObject player, int type)
    {
        float remainingTime = wattingTime;
        while (remainingTime > 0)
        {
            (type == 0 ? text1 : text2).text = remainingTime + "s";
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }
        (type == 0 ? text1 : text2).text = "1";

        player.transform.position = (player.name.Equals("Player1") ? spamPoint1.transform.position : spamPoint2.transform.position);
        propcontrol.ResetProp(player.GetComponent<PlayerMove>().isControllerPlayer ? 2 : 1);
        player.GetComponent<Player>().vulnerabilityIndex = 1;

       

        player.SetActive(true);

        // Make the player twinkle for 5 seconds
        Renderer playerRenderer = (player.name.Equals("Player1") ? animation1 : animation2).GetComponent<Renderer>();
        Color originalColor = playerRenderer.material.color;
        float twinkleDuration = 5f;
        float twinkleInterval = 0.2f; // Interval for twinkling effect


        Transform hitBox = player.transform.Find("HitBox");
        if (hitBox != null)
        {
            hitBox.gameObject.SetActive(false);

            // Run the twinkle effect
            for (float t = 0; t < twinkleDuration; t += twinkleInterval)
            {
                // Toggle transparency
                Color newColor = playerRenderer.material.color;
                newColor.a = newColor.a == 1f ? 0.3f : 1f; // Switch between opaque and semi-transparent
                playerRenderer.material.color = newColor;
                yield return new WaitForSeconds(twinkleInterval);
            }

            hitBox.gameObject.SetActive(true);
        }


        // Restore the original color
        playerRenderer.material.color = originalColor;
    }
}
