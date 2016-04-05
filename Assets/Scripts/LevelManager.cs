using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour {

	public GameObject[] bricks;
	public static int score;
	public Transform[] spawnPoints;
	protected int index;
	public int sum;
	public HashSet<Node> selectedNodes;
	public static bool isPaused = false;
	public GameObject bomb;
	public GameObject coin;
	public static bool isWatchedAds;

	public float timeToSpawn;
	protected float accumulator;

	protected void GetInput<T>() where T : Node{
		RaycastHit hit;
		if (Input.touchCount == 1 && !isPaused) {

			foreach (Touch touch in Input.touches) {
				if (Physics.Raycast (Camera.main.ScreenPointToRay (touch.position), out hit)) {
					selectedNodes.Add (hit.transform.gameObject.GetComponent<T> ());
				}

				switch (touch.phase) {
				case TouchPhase.Ended:
					{
						// the touch is ended so now we can calculate the time and distance
						int temp = 0;
						try{
							foreach (T node in selectedNodes) {
								temp += node.value;
							}
						}
						catch(NullReferenceException e){
							selectedNodes.Clear ();
						}
						if (temp == sum) {
							//TODO animation
							score += selectedNodes.Count;
							HowShouldINameThis ();
							UIManager.updateScore (score);
							foreach (T node in selectedNodes) {
								node.Destroy ();
							}
						} else {
							//TODO wrong sum animation 
						}
						temp = 0;
						selectedNodes = new HashSet<Node> ();
						break;
					}
				}
			}
		}
	}

	protected void HowShouldINameThis(){
		if (score >= 20 && score < 40)
			timeToSpawn = 1.6f;
		else if (score >= 40 && score < 60)
			timeToSpawn = 1.2f;
		else if (score >= 60 && score < 80)
			timeToSpawn = 0.8f;
		else if (score >= 80 && score < 100)
			timeToSpawn = 0.4f;
		else if (score >= 100)
			timeToSpawn = 0.2f;
	}

	protected void SpawnNodes(){
		accumulator -= Time.deltaTime;
		if (accumulator <= 0.0f) {
			if (!isPaused) {
				int temp = UnityEngine.Random.Range (0, 50);
				if (temp < 48)
					Instantiate (bricks [temp % 6], spawnPoints [index++].position, Quaternion.Euler (0, 180, 0));
				else if(temp == 48)
					Instantiate (bomb, spawnPoints [index++].position, Quaternion.Euler (0, 180, 0));
				else if(temp == 49)
					Instantiate (coin, spawnPoints [index++].position, Quaternion.Euler (0, 180, 0));
			}				
			accumulator = timeToSpawn;
			if (index == spawnPoints.Length)
				index = 0;
		}
	}

	public static void DeleteNodes(int amount){
		GameObject[] arr = GameObject.FindGameObjectsWithTag("Node");
		for (int i = arr.Length - 1; i >= arr.Length - 1 - amount; i--) {
			arr [i].GetComponent<Node> ().Destroy ();
		}
	}
}
