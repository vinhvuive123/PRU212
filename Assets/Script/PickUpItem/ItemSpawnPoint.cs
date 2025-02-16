using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int maxItems = 5; // Limit on the number of items allowed in the scene
    public int currentItemCount = 0; // Current number of items in the scene
    public float timeSpawn = 10f; // Time interval between spawns

    [Header("Range")]
    public float minX, maxX; // X coordinates for spawn area
    public float minY, maxY; // Y coordinates for spawn area

    [Header("Item Prefabs")]
    public List<GameObject> itemPrefabs; // List of item prefabs (e.g., ItemBomb, ItemSpikeBomb, ItemStickyBomb)

    [Header("Gravity Settings")]
    public float initialGravityScale = 0.5f; // Initial gravity scale
    public float normalGravityScale = 1f; // Gravity scale after hitting the ground

    private BoxCollider2D deadZoneCollider;

    private void Start()
    {
        StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator SpawnItemRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeSpawn);

            if (currentItemCount < maxItems)
            {
                SpawnRandomItem();
            }
        }
    }

    private void SpawnRandomItem()
    {

        // set location random span in range (x,y)
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        Vector2 spawnPosition = new Vector2(randomX, randomY);

        int randomIndex = Random.Range(0, itemPrefabs.Count);
        GameObject selectedItem = itemPrefabs[randomIndex];

        GameObject spawnedItem = Instantiate(selectedItem, spawnPosition, Quaternion.identity);
        currentItemCount++;


        Rigidbody2D rb = spawnedItem.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = initialGravityScale;
            // Assign the FallingItem script and set up the DeadZone boundaries
            FallingItem fallingItem = spawnedItem.AddComponent<FallingItem>();
            fallingItem.Setup(normalGravityScale);
        }
    }

    private void RemoveOldestItem()
    {
        GameObject[] allItems = GameObject.FindGameObjectsWithTag("Item");

        if (allItems.Length > 0)
        {
            Destroy(allItems[0]);
            currentItemCount--;
        }
    }

    public void DecreaseItemCount()
    {
        currentItemCount -= 1;
    }
    public void OnDestroy()
    {
        GameObject[] allItems = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject item in allItems)
        {
            Destroy(item);
        }
        currentItemCount = 0;
    }

    public class FallingItem : MonoBehaviour
    {
        private Rigidbody2D rb;
        private float normalGravityScale;

        public void Setup(float normalGravity)
        {
            rb = GetComponent<Rigidbody2D>();
            normalGravityScale = normalGravity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                rb.gravityScale = normalGravityScale;
            }
        }
    }
}
