using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TetrisLevelManager : LevelManager {

	public int height;
	public int width;

	void Start(){
		accumulator = timeToSpawn;
		selectedNodes = new HashSet<Node> ();
		score = 0;
		index = 0;
		InitMap ();
	}

	void Update(){
		SpawnNodes ();
		GetInput<Node> ();
	}

	void InitMap(){
		for(float r = 0; r < height; r++){
			for(float c = 0; c < width; c++){
				Instantiate (bricks[Random.Range (1,bricks.Length)], new Vector2 (c-2.5f, r-2), Quaternion.Euler(0, 180, 0));
			}
		}
	}

	void OnTriggerStay(Collider other){
		if (other.attachedRigidbody.velocity.magnitude == 0f)
			UIManager.ShowEndGamePage ();
	}
}
