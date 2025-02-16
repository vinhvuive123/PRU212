using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    [System.Serializable]
    public class DeadZoneProp
    {
        public Vector3 spamPoint1, spamPoint2;
        public Vector2 colliderOffset, colliderSize;
    }

    [System.Serializable]
    public class MapPrefab
    {
        public int id;
        public GameObject prefab; // Prefab for the map
        public Sprite bgSprite;
        public DeadZoneProp deadZone;
    }


    private PlayerMoveController inputControls;

    // property player
    [Header("I. Player input")]
    [SerializeField] private GameObject playerKeyBoard;
    [SerializeField] private GameObject playerController;
    [SerializeField] private GameObject playerKeyBoardRangePickUp, playerControllerRangePickUp;


    [Header("II. Life Info")]
    //private int life1;
    //private int life2;

    public int maxHealth = 100;

    //public int Life1 { get => playerKeyBoard.GetComponent<Player>().life; set => life1 = value; }
    //public int Life2 { get => playerController.GetComponent<Player>().life; set => life2 = value; }


    [Header("II. SkinNrId and MapId selected")]
    [SerializeField] private int playerId1;
    [SerializeField] private int playerId2;
    [Range(0, 5)]
    [SerializeField] private int mapId;


    [Header("III. Map controller")]
    [SerializeField] private GameObject groundAndPlatForm;
    [SerializeField] private GameObject backgroundImage;
    [SerializeField] private List<MapPrefab> mapPrefabs;

    [Header("IV. DeadZone Setup")]
    [SerializeField] private GameObject deadZone;
    //[SerializeField] private GameObject spampoint1, spampoint2;

    private Dictionary<int, MapPrefab> mapPrefabDictionary;

    void Awake()
    {
        mapPrefabDictionary = new Dictionary<int, MapPrefab>();
        foreach (MapPrefab map in mapPrefabs)
        {
            mapPrefabDictionary[map.id] = map;
        }

        playerId1 = GameInfo.Instance.player1ID;
        playerId2 = GameInfo.Instance.player2ID;
        mapId = GameInfo.Instance.mapID;

    }


    void Start()
    {
        // player prop  
        //Player info1 = playerKeyBoard.GetComponent<Player>(); 
        //Player info2 = playerController.GetComponent<Player>();
        //life1 = info1.life;
        //life2 = info2.life;



        // set controller
        inputControls = new();
        inputControls.Enable();


        playerController.GetComponent<PlayerMove>().Initialize(inputControls, isControllerPlayer: true);
        playerController.GetComponent<PlayerAttack>().Initialize(inputControls, isControllerPlayer: true);
        playerControllerRangePickUp.GetComponent<PickUpItem>().Initialize(inputControls, isControllerPlayer: true);

        playerKeyBoard.GetComponent<PlayerMove>().Initialize(inputControls, isControllerPlayer: false);
        playerKeyBoard.GetComponent<PlayerAttack>().Initialize(inputControls, isControllerPlayer: false);
        playerKeyBoardRangePickUp.GetComponent<PickUpItem>().Initialize(inputControls, isControllerPlayer: false);



        // set skin player 
        GameObject animationPlayerKeyBoard = playerKeyBoard.transform.Find("Animation")?.gameObject;
        GameObject animationPlayerController = playerKeyBoard.transform.Find("Animation")?.gameObject;

        animationPlayerKeyBoard.GetComponent<CustomizableCharacter>().SkinNr = playerId1;
        animationPlayerKeyBoard.GetComponent<CustomizableCharacter>().SkinNr = playerId2;

        // set map 
        GenerateMapById(mapId, groundAndPlatForm.transform.position, groundAndPlatForm.transform.rotation);



    }


    public void SetBeginPosition()
    {
        playerKeyBoard.transform.position = deadZone.GetComponent<DeadZone>().spamPoint1.transform.position;
        playerKeyBoard.SetActive(true);

        playerController.transform.position = deadZone.GetComponent<DeadZone>().spamPoint2.transform.position;
        playerController.SetActive(true);
    }

    public GameObject GenerateMapById(int id, Vector3 position, Quaternion rotation)
    {
        if (mapPrefabDictionary.TryGetValue(id, out MapPrefab mapPrefab))
        {
            // set background in BackgroundImage
            backgroundImage.GetComponent<UnityEngine.UI.Image>().sprite = mapPrefabDictionary[id].bgSprite;

            // Instantiate the map prefab at the specified position and rotation
            GameObject mapInstance = Instantiate(mapPrefab.prefab, position, rotation);
            mapInstance.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);

            deadZone.GetComponent<BoxCollider2D>().offset = mapPrefab.deadZone.colliderOffset;
            deadZone.GetComponent<BoxCollider2D>().size = mapPrefab.deadZone.colliderSize;
            deadZone.GetComponent<DeadZone>().spamPoint1.transform.position = mapPrefab.deadZone.spamPoint1;
            deadZone.GetComponent<DeadZone>().spamPoint2.transform.position = mapPrefab.deadZone.spamPoint2;

            //Debug.Log("post1" + deadZone.GetComponent<DeadZone>().spamPoint1.transform.position);
            //Debug.Log("post2" + deadZone.GetComponent<DeadZone>().spamPoint2.transform.position);


            return mapInstance;
        }
        else
        {
            Debug.LogWarning($"Map with ID {id} not found!");
            return null;
        }
    }

    private void OnDestroy()
    {
        inputControls.Disable();
    }
}
