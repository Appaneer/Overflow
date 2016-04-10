using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour {

	public GameObject[] bricks;
	public static int score;
	public Transform[] spawnPoints;
	protected int index;
	public static int sum;
	public HashSet<Node> selectedNodes;
	public static bool isPaused = false;
	public GameObject bomb;
	public GameObject coin;
	public static bool isWatchedAds;
	public static AudioSource audio;
	public float timeToSpawn;
	protected float accumulator;
	public int temp=0;
	void Start()
	{
		audio = GetComponent<AudioSource> ();
	}

	public static void SetSum(int s){
		sum = s;
	}

	protected void GetInput<T>() where T : Node{
		RaycastHit hit;
		if (Input.touchCount == 1 && !isPaused) {
			foreach (Touch touch in Input.touches) {
				if (Physics.Raycast (Camera.main.ScreenPointToRay (touch.position), out hit)) {
					selectedNodes.Add (hit.transform.gameObject.GetComponent<T> ());

					string childLocation =hit.transform.gameObject + "/Quad";
					GameObject childObject = GameObject.Find (childLocation);
					childObject.GetComponent<MeshRenderer>().enabled=true;
				}

				switch (touch.phase) {
				case TouchPhase.Ended:
					{
						// the touch is ended so now we can calculate the time and distance
						temp = 0;
						try{
							foreach (T node in selectedNodes) {
								string childLocation =node + "/Quad";
								GameObject childObject = GameObject.Find (childLocation);
								childObject.GetComponent<MeshRenderer>().enabled=false;
								temp += node.value;
							}
						}
						catch(NullReferenceException e){
							Debug.Log (e.Message);
							selectedNodes.Clear ();
						}

						if (temp == sum) {
							score += selectedNodes.Count;
							timeToSpawn = -0.01f * score + 2f;//using an equation to model this y = -0.01x + 2(y is timeToSpawn and x is score)
							UIManager.updateScore (score);
							foreach (T node in selectedNodes) {
								node.Destroy ();
								audio.Play ();
							}
						} else {
							//TODO wrong sum animation 
							UIManager.TriggerDisplaySumText (temp);
						}
						temp = 0;
						selectedNodes = new HashSet<Node> ();
						break;
					}
				}
			}
		}
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
