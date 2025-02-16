using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomizableCharacter : MonoBehaviour
{

    [SerializeField]
    [Range(0, 3)]
    private int skinNr;

    public int SkinNr { get => skinNr; set => skinNr = value; }



    public Skins[] skins;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SkinChoice();
    }
    void SkinChoice()
    {
        if (spriteRenderer.sprite.name.Contains("Jake"))
        {
            string spriteName = spriteRenderer.sprite.name;
            spriteName = spriteName.Replace("Jake_", "");
            // get number of current sprite
            int spriteNr = int.Parse(spriteName);

            spriteRenderer.sprite = skins[skinNr].sprites[spriteNr];
        }
    }
}
[System.Serializable]
public struct Skins
{
    public Sprite[] sprites;
}
