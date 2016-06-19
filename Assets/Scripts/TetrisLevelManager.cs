using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TetrisLevelManager : LevelManager {

	public int height;
	public int width;

	public GameObject platform;
	public bool tempBool = false;

	/// <summary>
	/// The number of nodes in columns.
	/// [0] indicates the # of nodes in the first column.
	/// </summary>
	public static int[] numberOfNodesInCol;

	void Start(){
		Initialization ();
		print (PlayerPrefs.GetInt ("NextSum"));
		numberOfNodesInCol = new int[] {5,5,5,5,5,5};
		InitMap ();
	}

	void Update(){
		if (!isShowedTutorial && PlayerPrefs.GetInt ("Coins") == 0) {
			StartCoroutine ("wait");
		}
		SpawnNodes ();
		GetInput<Node> ();
	}

	IEnumerator wait(){
		UIManager.instance.tutorialCanvas.enabled = true;
		platform.GetComponent<Renderer>().enabled = false;
		yield return new WaitForSeconds (3.0f);
		isShowedTutorial = true;
		UIManager.instance.tutorialCanvas.enabled = false;
		platform.GetComponent<Renderer>().enabled = true;
	}

	void InitMap(){
		for(float r = 0; r < height; r++){
			for(float c = 0; c < width; c++){
				GameObject tempNode = Instantiate (bricks[Random.Range (1,bricks.Length)], new Vector2 (c-2.5f, r-2), Quaternion.Euler(0, 180, 0)) as GameObject;
				tempNode.GetComponent<Node> ().col = ((int)c)+1;
			}
		}
	}

	public override void SpawnNodes (){
		accumulator -= Time.deltaTime;
		if (accumulator <= 0.0f) {
			if (!isPaused) {
				index = 0;
				int min = numberOfNodesInCol [0];
				for(int i = 0; i < numberOfNodesInCol.Length; i++ ){
					if (numberOfNodesInCol [i] < min) {
						index = i;
						min = numberOfNodesInCol [i];
					}
					else if (numberOfNodesInCol [i] == min) {
						if (Random.Range (0, 3) == 0) {
							index = i;
							min = numberOfNodesInCol [i];
						}
					}
				}
				int temp = UnityEngine.Random.Range (0, 50);
				GameObject hii;
				if (temp < 48)
					hii = Instantiate (bricks [temp % 6], spawnPoints [index].position, Quaternion.Euler (0, 180, 0)) as GameObject;
				else if(temp == 48)
					hii = Instantiate (bomb, spawnPoints [index].position, Quaternion.Euler (0, 180, 0)) as GameObject;
				else
					hii = Instantiate (coin, spawnPoints [index].position, Quaternion.Euler (0, 180, 0)) as GameObject;
				hii.GetComponent<Node> ().col = index + 1;
				++TetrisLevelManager.numberOfNodesInCol [index];
			}				
			accumulator = timeToSpawn;
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
