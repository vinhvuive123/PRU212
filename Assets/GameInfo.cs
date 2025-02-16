using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
	private static GameInfo _instance;

	// Publicly accessible instance property
	public static GameInfo Instance
	{
		get
		{
			// If instance doesn't exist, try to find it in the scene
			if (_instance == null)
			{
				_instance = FindFirstObjectByType<GameInfo>();

				// If still null, create a new GameObject and add GameInfo to it
				if (_instance == null)
				{
					GameObject singleton = new GameObject("GameInfo");
					_instance = singleton.AddComponent<GameInfo>();
					DontDestroyOnLoad(singleton);
				}
			}
			return _instance;
		}
	}

	// Variables to store game information
	public int mapID;
	public int player1ID;
	public int player2ID;

    // Example data members
    public int winnerPlayerID;
    public int loserPlayerID;
    public int winnerNumber;


    // Optional: Awake to enforce the singleton pattern
    void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	// Example method to reset game info
	public void ResetGameInfo()
	{
		mapID = -1;
		player1ID = -1;
		player2ID = -1;
	}
}
