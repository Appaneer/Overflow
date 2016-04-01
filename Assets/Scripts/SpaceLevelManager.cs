using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceLevelManager : LevelManager {

	void Start(){
		accumulator = timeToSpawn;
		score = 0;
		index = 0;
		selectedNodes = new HashSet<Node> ();
		isWatchedAds = false;
		InitMap ();
	}

	void Update(){
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
