using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceLevelManager : LevelManager {

	float timeToChange;
	float acc;
	public int target;

	void Start(){
		timeToChange = 10.0f;
		LevelManager.setSum (10);
		UIManager.updateProduct (10);

		isPaused = false;
		score = 0;
		index = 0;
		accumulator = timeToSpawn;
		selectedNodes = new HashSet<Node> ();
		isWatchedAds = false;
		InitMap ();

		acc = timeToChange;

	}

	void Update(){
		acc-= Time.deltaTime;
		if (acc <= 0.0f) {
			target= Random.Range (10, 20);
			LevelManager.setSum (target);
			UIManager.updateProduct (target);

			timeToChange = Random.Range (10.0f, 15.0f);
			acc = timeToChange;
		}

		SpawnNodes ();
		GetInput<Node> ();
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
