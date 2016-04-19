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
		audioSource = GetComponent<AudioSource> ();
		if (PlayerPrefs.GetInt ("NextSum") == 0)
			SetSum (UnityEngine.Random.Range (10, 19));
		else
			SetSum (PlayerPrefs.GetInt ("NextSum"));
		UIManager.UpdateSumText (sum);
		isPaused = false;
		for(int number = 1; number <= 6; number++){
			UIManager.updateText (GameObject.Find(number+" text").GetComponent<Text>(), PlayerPrefs.GetInt("Num"+number));
		}
		UIManager.updateText (GameObject.Find("horizontal text").GetComponent<Text>(), PlayerPrefs.GetInt("NumHor"));
		UIManager.updateText (GameObject.Find("vertical text").GetComponent<Text>(), PlayerPrefs.GetInt("NumVer"));
		UIManager.updateText (GameObject.Find("bomb text").GetComponent<Text>(), PlayerPrefs.GetInt("NumBomb"));
		accumulator = timeToSpawn;
		selectedNodes = new HashSet<Node> ();
		score = 0;
		index = 0;
		isWatchedAds = false;
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
		if (index == spawnPoints.Length - 1)
			temp = 0;
		else
			temp = index + 1;
		switch (powerups) {
		case "horizontal":
			if (PlayerPrefs.GetInt ("NumHor") <= 0)
				return;
			Instantiate (horizontal, spawnPoints [temp].position, Quaternion.Euler (0, 180, 0));
			PlayerPrefs.SetInt ("NumHor", PlayerPrefs.GetInt("NumHor") - 1);
			UIManager.updateText (GameObject.Find("horizontal text").GetComponent<Text>(), PlayerPrefs.GetInt("NumHor"));
			break;

		case "vertical":
			if (PlayerPrefs.GetInt ("NumVer") <= 0)
				return;
			Instantiate (vertical, spawnPoints [temp].position, Quaternion.Euler (0, 180, 0));
			PlayerPrefs.SetInt ("NumVer", PlayerPrefs.GetInt("NumVer") - 1);
			UIManager.updateText (GameObject.Find("vertical text").GetComponent<Text>(), PlayerPrefs.GetInt("NumVer"));
			break;

		case "bomb":
			if (PlayerPrefs.GetInt ("NumBomb") <= 0)
				return;
			Instantiate (bomb, spawnPoints [temp].position, Quaternion.Euler (0, 180, 0));
			PlayerPrefs.SetInt ("NumBomb", PlayerPrefs.GetInt("NumBomb") - 1);
			UIManager.updateText (GameObject.Find("bomb text").GetComponent<Text>(), PlayerPrefs.GetInt("NumBomb"));
			break;
		}
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

	void OnTriggerStay(Collider other){
		if (other.attachedRigidbody.velocity.magnitude <= 0.1f)
			UIManager.ShowEndGamePage ();
	}
}
