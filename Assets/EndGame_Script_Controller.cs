using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame_Script_Controller : MonoBehaviour
{
	// Start is called before the first frame update
	[SerializeField]
	public List<AudioClip> listClip;
	[SerializeField]
    List<AnimatorController> listAnimatorWinner;
	[SerializeField]
	List<AnimatorController> listAnimatorLoser;
    [SerializeField]
    Image winnerImage;
	[SerializeField]
	Image loserImage;
	[SerializeField]
	Text winnerText;
	[SerializeField]
	Text loserText;
	int player_Winner ;
	int loserPlayer;
    int winnerPlayer;
	private AudioSource audioSource;
	void Start()
	{

		// gan du lieu cho Instance de test
		GameInfo.Instance.winnerNumber =0;
		GameInfo.Instance.winnerPlayerID =1;
		GameInfo.Instance.loserPlayerID = 2;


		player_Winner = GameInfo.Instance.winnerNumber;
		winnerPlayer= GameInfo.Instance.winnerPlayerID;
		loserPlayer = GameInfo.Instance.loserPlayerID;
		// gan nhac theo nhan vat chien thang 
		audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.clip = listClip[winnerPlayer];
		audioSource.Play();

		if (player_Winner == 0)
		{
			winnerText.text = "P1 Win";
			loserText.text = "P2 Lose";

		}
		else
		{
			winnerText.text = "P2 Win";
			loserText.text = "P1 Lose";
		}
		// Gán Animator cho winnerImage
		Animator animatorWinner = winnerImage.gameObject.GetComponent<Animator>();
		if (animatorWinner == null)
		{
			animatorWinner = winnerImage.gameObject.AddComponent<Animator>();
		}

		// Kiểm tra danh sách Animator và gán controller từ listAnimatorWinner vào Animator của winnerImage
		if (listAnimatorWinner != null && listAnimatorWinner.Count > 0)
		{
			animatorWinner.runtimeAnimatorController = listAnimatorWinner[winnerPlayer];
		}
		else
		{
			Debug.LogWarning("listAnimatorWinner rỗng hoặc chưa được khởi tạo.");
		}

		// Gán Animator cho loserImage
		Animator animatorLoser = loserImage.gameObject.GetComponent<Animator>();
		if (animatorLoser == null)
		{
			animatorLoser = loserImage.gameObject.AddComponent<Animator>();
		}

		// Kiểm tra danh sách Animator và gán controller từ listAnimatorLoser vào Animator của loserImage
		if (listAnimatorLoser != null && listAnimatorLoser.Count > 0)
		{
			animatorLoser.runtimeAnimatorController = listAnimatorLoser[loserPlayer];
		}
		else
		{
			Debug.LogWarning("listAnimatorLoser rỗng hoặc chưa được khởi tạo.");
		}
	}
	public void RestartGameButton()
	{
		audioSource.Stop();
		if (GameInfo.Instance != null)
		{
			GameInfo.Instance.ResetGameInfo();
		}
		SceneManager.LoadScene(0);
	}
}