using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundLoop : MonoBehaviour
{
    [SerializeField]
    private RawImage background;
    [SerializeField]
    private float x, y;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        background.uvRect = new Rect(background.uvRect.position + new Vector2(x, y) * Time.deltaTime, background.uvRect.size);
    }
}
