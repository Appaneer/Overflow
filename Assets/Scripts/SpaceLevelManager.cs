using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceLevelManager : LevelManager {

	void Start(){
		accumulator = timeToSpawn;
		score = 0;
		index = 0;
		selectedNodes = new HashSet<Node> ();
		InitMap ();
	}

	void Update(){
		SpawnNodes ();
		GetInput<Node> ();
	}

	void InitMap(){
		for(float r = 2; r >= -2; r--){
			for(float c = 2; c >= -2; c--){
				Instantiate (bricks[Random.Range (1,bricks.Length)], new Vector2 (r, c), Quaternion.identity);
			}
		}
	}

	void OnTriggerExit(Collider other){
		UIManager.ShowEndGamePage ();
	}
}
