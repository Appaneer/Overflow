using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceLevelManager : LevelManager {

	void Start(){
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
		if (!isShowedTutorial && PlayerPrefs.GetInt ("Coins") == 0)
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
