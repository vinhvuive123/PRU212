using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;
using UnityEngine.EventSystems;
using static Unity.Collections.AllocatorManager;
using UnityEngine.SceneManagement;

public class ButtonAnimationControllerScript : MonoBehaviour
{
	[Header("I. Select Map Stage")]
	[SerializeField] Text mapNameText;
	[SerializeField] GameObject selectMap_Stage;
	[SerializeField] UnityEngine.UI.Button NextToPlayGameButton;
	[SerializeField] List<Sprite> listSpriteMap;
	[SerializeField] string[] listNameMap;
	[SerializeField] Canvas Canvas_SelectMap;

	[Header("II. Select Player Stage")]
	[SerializeField] GameObject selectPlayer_Stage;
	[SerializeField] UnityEngine.UI.Button nextStage;
	[SerializeField] Text stateText;
	[Header("a. list sprite")]
	[SerializeField] List<Sprite> listSpriteSelect;
	[SerializeField] List<Sprite> listSpriteAvatar;
	[Header("b. image")]
	[SerializeField] Image AnimationAfterSelect;
	[SerializeField] Image Player1_Avatar;
	[SerializeField] Image Player2_Avatar;
	//[SerializeField]
	//public Animator buttonAnimator;

	[Header("c. button select")]
	[SerializeField] UnityEngine.UI.Button buttonSelectCharacter;
	[SerializeField] UnityEngine.UI.Button BeDungTrai;
	[SerializeField] UnityEngine.UI.Button BeDungPhai;
	[SerializeField] public List<RuntimeAnimatorController> animatorControllers;

	int indexMap = 0;
	int index = 0;
	int click = 0;
	int map = 0;

	int player1 = 0;
	int player2 = 0;

	Color colorDoTrongSuot ;
	private void Start()
	{
		//active select character stage. inactive select map stage
		selectPlayer_Stage.SetActive(true);
		selectMap_Stage.SetActive(false);
		//truyen animator cho 3 thang 
		buttonSelectCharacter.animator.runtimeAnimatorController = animatorControllers[index];
		BeDungTrai.animator.runtimeAnimatorController = animatorControllers[GetIndexLeft(index, animatorControllers.Count)];
		BeDungPhai.animator.runtimeAnimatorController = animatorControllers[GetIndexRight(index, animatorControllers.Count)];
		stateText.text = "Select Player 1";
		//AnimationAfterSelect.sprite = listSpriteSelect[index];
		//color set trong suot
		colorDoTrongSuot = new Color(255.0f,255.0f,255.0f);
		colorDoTrongSuot.a = 0f;
		AnimationAfterSelect.color= colorDoTrongSuot;
		Player1_Avatar.color = colorDoTrongSuot;
		Player2_Avatar.color = colorDoTrongSuot;
		//tat tuong tac va hien thi cua cac nut next stage
		nextStage.GetComponent<Image>().color = colorDoTrongSuot;
		nextStage.interactable = false;

		NextToPlayGameButton.GetComponent<Image>().color = colorDoTrongSuot;
		NextToPlayGameButton.interactable = false;

		Canvas_SelectMap.GetComponent<Image>().sprite = listSpriteMap[indexMap];
		mapNameText.text = listNameMap[indexMap];


	}

	public void ChangeAnimatorForward()
	{
		if (index == (animatorControllers.Count-1))
		{
			index = 0;
		}
		else
		{
			index++;
		}
		buttonSelectCharacter.animator.runtimeAnimatorController = animatorControllers[index];
		BeDungTrai.animator.runtimeAnimatorController = animatorControllers[GetIndexLeft(index, animatorControllers.Count)];
		BeDungPhai.animator.runtimeAnimatorController = animatorControllers[GetIndexRight(index, animatorControllers.Count)];
	}

	public void ChangeAnimatorBackward()
	{
		if (index == 0)
		{
			index = animatorControllers.Count-1;
		}
		else
		{
			index--;
		}
		buttonSelectCharacter.animator.runtimeAnimatorController = animatorControllers[index];
		BeDungTrai.animator.runtimeAnimatorController = animatorControllers[GetIndexLeft(index, animatorControllers.Count)];
		BeDungPhai.animator.runtimeAnimatorController = animatorControllers[GetIndexRight(index, animatorControllers.Count)];

	}
	public void SelectPlayer()
	{
		click++;
		if (click == 1)
		{
			//buttonSelectCharacter.interactable = false; 
			Debug.Log("1 - " + index);
			StartCoroutine(ShowAndHideUI(AnimationAfterSelect, 2));
			// gan sprite va hien thi avatar1
			stateText.text = "Select Player 2";
			Player1_Avatar.sprite = listSpriteAvatar[index];

			colorDoTrongSuot.a = 255.0f;
			Player1_Avatar.color = colorDoTrongSuot;
			player1 = index;
			GameInfo.Instance.player1ID = index;
		}
		if (click ==2)
		{
			Debug.Log("2 - "+ index);
			StartCoroutine(ShowAndHideUI(AnimationAfterSelect, 2));
			// gan sprite va hien thi avatar2
			stateText.text = "Ready To Select Map";
			Player2_Avatar.sprite = listSpriteAvatar[index];
			colorDoTrongSuot.a = 255.0f;
			Player2_Avatar.color = colorDoTrongSuot;
			player2 = index;
			//sau khi chon xong 2 nhan vat, hien thi nut next stage va kich hoat no
			nextStage.GetComponent<Image>().color = colorDoTrongSuot;
			nextStage.interactable = true;
			GameInfo.Instance.player2ID = index;
		}
	}
	public int GetIndexLeft(int i,int countList)
	{
		if (i == 0)
		{
			i = countList - 1;
		}
		else
		{
			i--;
		}
		return i;
	}
	public int GetIndexRight(int i, int countList)
	{
		if (i == (countList - 1))
		{
			i = 0;
		}
		else
		{
			i++;
		}
		return i;
	}
	public void BackStepButtonOnPress()
	{
		//if(click)
		if (stateText.text.Equals("Ready To Select Map"))
		{
			click = 1;
			player2 = -1;
			stateText.text = "Select Player 2";
			colorDoTrongSuot.a = 0f;
			Player2_Avatar.color = colorDoTrongSuot;
			// thiet lap lai nut next k cho hien thi va tuong tac nua
			nextStage.GetComponent<Image>().color = colorDoTrongSuot;
			nextStage.interactable = false;

		}
		else if (stateText.text.Equals("Select Player 2"))
		{
			click = 0;
			player1 = -1;
			stateText.text = "Select Player 1";
			colorDoTrongSuot.a = 0f;
			Player1_Avatar.color = colorDoTrongSuot;
		} else if (stateText.text.Equals("Select Player 1"))
		{

		}

	}
	public void NextStepButtonOnPress()
	{
		selectPlayer_Stage.SetActive(false);
		selectMap_Stage.SetActive(true);
	}
	public void NextMapButtonOnPress()
	{
		if (indexMap == (listSpriteMap.Count - 1))
		{
			indexMap = 0;
		}
		else
		{
			indexMap++;
		}
		Canvas_SelectMap.GetComponent<Image>().sprite = listSpriteMap[indexMap];
		mapNameText.text = listNameMap[indexMap];

	}
	public void BackToSelectCharacterButton()
	{
		if (map!=-1)
		{
			colorDoTrongSuot.a = 0f;
			NextToPlayGameButton.GetComponent<Image>().color = colorDoTrongSuot;
			nextStage.interactable = false;
			map = -1;
		}
		else
		{

			selectPlayer_Stage.SetActive(true);
			selectMap_Stage.SetActive(false);
		}

	}

	public void BackMapButtonOnPress()
	{
		if (indexMap == 0)
		{
			indexMap = listSpriteMap.Count - 1;
		}
		else
		{
			indexMap--;
		}
		Canvas_SelectMap.GetComponent<Image>().sprite = listSpriteMap[indexMap];
		mapNameText.text = listNameMap[indexMap];
	}
	public void SelectMapButtonOnPress()
	{
		map = indexMap;
		// hien thi va cho phep tuong tac voi nut khoi dong game
		colorDoTrongSuot.a = 255.0f;
		NextToPlayGameButton.GetComponent<Image>().color = colorDoTrongSuot;
		NextToPlayGameButton.interactable = true;
		GameInfo.Instance.mapID = indexMap;
	}
	public void NextToPlayGameButtonOnPress()
	{
		GameInfo.Instance.mapID = map;
		GameInfo.Instance.player1ID = player1;
		GameInfo.Instance.player2ID = player2;
		SceneManager.LoadScene(2);
	}
	IEnumerator ShowAndHideUI(Image AnimationAfterSelect, float delay)
	{
		buttonSelectCharacter.interactable = false;
		//Bat trigger animation
		buttonSelectCharacter.animator.SetTrigger("Selected");

		// Hien Thi background select player
		AnimationAfterSelect.sprite = listSpriteSelect[index];
		colorDoTrongSuot.a = 255.0f;
		AnimationAfterSelect.color = colorDoTrongSuot;
		yield return new WaitForSeconds(delay);
		// tat background select player
		colorDoTrongSuot.a = 0f;
		AnimationAfterSelect.color = colorDoTrongSuot;
		buttonSelectCharacter.interactable = true;

	}

}
