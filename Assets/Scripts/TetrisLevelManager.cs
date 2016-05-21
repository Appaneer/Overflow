using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TetrisLevelManager : LevelManager {

	public int height;
	public int width;

	public GameObject horizontal;
	public GameObject vertical;

	/// <summary>
	/// The number of nodes in columns.
	/// [0] indicates the # of nodes in the first column.
	/// </summary>
	public static int[] numberOfNodesInCol;

	void Start(){
		numberOfNodesInCol = new int[] {5,5,5,5,5,5};
		audioSource = GetComponent<AudioSource> ();
		SetSum (UnityEngine.Random.Range (10, 19));
		UIManager.UpdateSumText (sum);
		isPaused = false;
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
				}
				int temp = UnityEngine.Random.Range (0, 49);
				GameObject hii;
				if (temp < 48)
					hii = Instantiate (bricks [temp % 6], spawnPoints [index].position, Quaternion.Euler (0, 180, 0)) as GameObject;
				else
					hii = Instantiate (bomb, spawnPoints [index].position, Quaternion.Euler (0, 180, 0)) as GameObject;
				hii.GetComponent<Node> ().col = index + 1;
				++TetrisLevelManager.numberOfNodesInCol [index];
			}				
			accumulator = timeToSpawn;
		}
	}

	void OnTriggerStay(Collider other){
		if (other.attachedRigidbody.velocity.magnitude <= 0.1f)
			UIManager.ShowEndGamePage ();
	}
}
