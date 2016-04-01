using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TetrisLevelManager : LevelManager {

	public int height;
	public int width;

	public GameObject horizontal;
	public GameObject vertical;

	void Start(){
		UIManager.updateText (GameObject.Find("horizontal text").GetComponent<Text>(), PlayerPrefs.GetInt("NumHor"));
		UIManager.updateText (GameObject.Find("vertical text").GetComponent<Text>(), PlayerPrefs.GetInt("NumVer"));
		UIManager.updateText (GameObject.Find("bomb text").GetComponent<Text>(), PlayerPrefs.GetInt("NumBomb"));
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

	public void SpawnPowerups(string powerups){
		int temp;
		if (index == 0)
			temp = spawnPoints.Length - 1;
		else if (index == 1)
			temp = 0;
		else
			temp = index - 2;
		switch (powerups) {
		case "horizontal":
			Instantiate (horizontal, spawnPoints [temp].position, Quaternion.Euler (0, 180, 0));
			PlayerPrefs.SetInt ("NumHor", PlayerPrefs.GetInt("NumHor") - 1);
			UIManager.updateText (GameObject.Find("horizontal text").GetComponent<Text>(), PlayerPrefs.GetInt("NumHor"));
			break;

		case "vertical":
			Instantiate (vertical, spawnPoints [temp].position, Quaternion.Euler (0, 180, 0));
			PlayerPrefs.SetInt ("NumVer", PlayerPrefs.GetInt("NumVer") - 1);
			UIManager.updateText (GameObject.Find("vertical text").GetComponent<Text>(), PlayerPrefs.GetInt("NumVer"));
			break;

		case "bomb":
			Instantiate (bomb, spawnPoints [temp].position, Quaternion.Euler (0, 180, 0));
			PlayerPrefs.SetInt ("NumBomb", PlayerPrefs.GetInt("NumBomb") - 1);
			UIManager.updateText (GameObject.Find("bomb text").GetComponent<Text>(), PlayerPrefs.GetInt("NumBomb"));
			break;
		}
	}

	void OnTriggerStay(Collider other){
		if (other.attachedRigidbody.velocity.magnitude == 0f)
			UIManager.ShowEndGamePage ();
	}
}
