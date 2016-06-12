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

	public static bool isAudioOn;
	public AudioClip coinSFX;
	public AudioClip popSFX;
	public AudioClip clickSFX;
	AudioClip sfx;
	protected AudioSource audioSource;

	private int currentSum = 0;

	protected bool isShowedTutorial = false;//was the tutorial page showed?
	//this is abstract class, it shouldn't have Start() or Update()

	/// <summary>
	/// Sets the sum.
	/// </summary>
	/// <param name="s">Sum</param>
	public static void SetSum(int s){
		sum = s;
	}

	protected void GetInput<T>() where T : Node{
		RaycastHit hit;
		if (Input.touchCount == 1 && !isPaused) {
			foreach (Touch touch in Input.touches) {
				if (Physics.Raycast (Camera.main.ScreenPointToRay (touch.position), out hit)) {
					T tempNode = hit.transform.gameObject.GetComponent<T> ();
					try{
						tempNode.DisplayQuad ();
						if(selectedNodes.Add (tempNode) && isAudioOn)
							audioSource.PlayOneShot (clickSFX);
					}
					catch(NullReferenceException e){
						e.ToString ();
					}
				}
				currentSum = GetCurrentSum ();
				UIManager.UpdateCurrentSum (currentSum);
				switch (touch.phase) {
				case TouchPhase.Ended:
					{
						UIManager.DisableSumText ();
						// the touch is ended so now we can calculate the time and distance
						try{
							sfx = popSFX;
							foreach (T node in selectedNodes) {
								if(node.value == 0)
									sfx = coinSFX;
								node.HideQuad();
							}
						}
						catch(NullReferenceException e){
							Debug.Log (e.Message);
							selectedNodes.Clear ();
						}

						if (currentSum == sum) {
							if(isAudioOn)
								audioSource.PlayOneShot (sfx);
							score += selectedNodes.Count;
							if(score < 70){
								timeToSpawn = -0.015f * score + 2f;//using an equation to model this y = -0.015x + 2(y is timeToSpawn and x is score)
							}
							UIManager.updateScore (score);
							foreach (T node in selectedNodes) {
								node.Destroy ();
							}
						}
						currentSum = 0;
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

	public int GetCurrentSum(){
		int sum = 0;
		foreach (Node n in selectedNodes) {
			sum += n.value;
		}
		return sum;
	}
}
