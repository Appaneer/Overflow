using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpaceLevelManager : LevelManager {

	void Start(){
		for(int number = 1; number <= 6; number++){
			UIManager.updateText (GameObject.Find(number+" text").GetComponent<Text>(), PlayerPrefs.GetInt("Num"+number));
		}
		UIManager.updateText (GameObject.Find("freeze").GetComponent<Text>(), PlayerPrefs.GetInt("Freeze"));
		isAudioOn = PlayerPrefs.GetInt ("isAudioOn") == 0;//0 = true = audio is on, 1 = false = audio is off
		audioSource = GetComponent<AudioSource> ();
		isPaused = false;
		accumulator = timeToSpawn;
		score = 0;
		index = 0;
		selectedNodes = new HashSet<Node> ();
		isWatchedAds = false;
		InitMap ();
		if (PlayerPrefs.GetInt ("NextSum") == 0)
			SetSum (UnityEngine.Random.Range (10, 19));
		else
			SetSum (PlayerPrefs.GetInt ("NextSum"));
		UIManager.UpdateSumText (sum);
	}

	void Update(){
		if (!isShowedTutorial && PlayerPrefs.GetInt ("Coins") == 0 &&  UIManager.instance.tutorialCanvas!=null)
			StartCoroutine ("wait");
		SpawnNodes ();
		GetInput<Node> ();
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
		for(float r = 2f; r >= -2f; r--){
			for(float c = 2f; c >= -2f; c--){
				Instantiate (bricks[Random.Range (1,bricks.Length)], new Vector2 (r, c), Quaternion.Euler(0, 180, 0));
			}
		}
	}

	void OnTriggerExit(Collider other){
		UIManager.ShowEndGamePage ();
	}
}
