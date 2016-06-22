using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpaceLevelManager : LevelManager {

	void Start(){
		Initialization ();
		InitMap ();

		if (!isShowedTutorial && PlayerPrefs.GetInt ("Coins") == 0) {
			UIManager.instance.tutorialCanvas.enabled = true;
		}
	}

	void Update(){
		if (isShowedTutorial) {
			SpawnNodes ();
			GetInput<Node> ();
		} else if (!isShowedTutorial && Input.GetMouseButtonDown (0)) {
			isShowedTutorial = true;
			UIManager.instance.tutorialCanvas.enabled = false;
		}
	}

	IEnumerator wait(){
		UIManager.instance.tutorialCanvas.enabled = true;
		yield return new WaitForSeconds (3.0f);
		isShowedTutorial = true;
		UIManager.instance.tutorialCanvas.enabled = false;
	}

	public void UseNumberPowerups(int number){
		if (PlayerPrefs.GetInt ("Num" + number) <= 0)
			return;
		GameObject[] arr = GameObject.FindGameObjectsWithTag ("Node");
		PlayerPrefs.SetInt ("Num"+number, PlayerPrefs.GetInt("Num"+number) - 1);
		UIManager.updateText (GameObject.Find(number+" text").GetComponent<Text>(), PlayerPrefs.GetInt("Num"+number));
		foreach(GameObject i in arr){
			Node tempNode = i.GetComponent<Node> ();
			if (tempNode.value == number)
				tempNode.Destroy ();
		}
	}

	void InitMap(){
		for(float r = 2.5f; r >= -2.5f; r--){
			for(float c = 2.5f; c >= -2.5f; c--){
				Instantiate (bricks[Random.Range (1,bricks.Length)], new Vector2 (r, c), Quaternion.Euler(0, 180, 0));
			}
		}
	}

	void OnTriggerExit(Collider other){
		UIManager.ShowEndGamePage ();
	}
}
