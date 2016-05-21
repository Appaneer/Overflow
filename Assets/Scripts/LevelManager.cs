using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class LevelManager : MonoBehaviour {

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
	public float timeToSpawn;
	protected float accumulator;

	public AudioClip coinSFX;
	public AudioClip popSFX;
	protected AudioSource audioSource;

	public static void SetSum(int s){
		sum = s;
	}

	protected void GetInput<T>() where T : Node{
		RaycastHit hit;
		if (Input.touchCount == 1 && !isPaused) {
			foreach (Touch touch in Input.touches) {
				if (Physics.Raycast (Camera.main.ScreenPointToRay (touch.position), out hit)) {
					T tempNode = hit.transform.gameObject.GetComponent<T> ();
					tempNode.DisplayQuad ();
					selectedNodes.Add (tempNode);
				}

				switch (touch.phase) {
				case TouchPhase.Ended:
					{
						// the touch is ended so now we can calculate the time and distance
						int temp = 0;
						try{
							foreach (T node in selectedNodes) {
								temp += node.value;
								node.HideQuad();
							}
						}
						catch(NullReferenceException e){
							Debug.Log (e.Message);
							selectedNodes.Clear ();
						}

						if (temp == sum) {
							audioSource.PlayOneShot (popSFX);
							score += selectedNodes.Count;
							timeToSpawn = -0.01f * score + 2f;//using an equation to model this y = -0.02x + 2(y is timeToSpawn and x is score)
							UIManager.updateScore (score);
							foreach (T node in selectedNodes) {
								node.Destroy ();
							}
						} else {
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

	public virtual void SpawnNodes (){
		accumulator -= Time.deltaTime;
		if (accumulator <= 0.0f) {
			if (!isPaused) {
				int temp = UnityEngine.Random.Range (0, 49);
				if (temp < 48)
					Instantiate (bricks [temp % 6], spawnPoints [index++].position, Quaternion.Euler (0, 180, 0));
				else if(temp == 48)
					Instantiate (bomb, spawnPoints [index++].position, Quaternion.Euler (0, 180, 0));
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
