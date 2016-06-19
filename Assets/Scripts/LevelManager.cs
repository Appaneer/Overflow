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
	public static bool isInputDisable;
	public GameObject bomb;
	public GameObject coin;
	public static bool isWatchedAds;
	public float timeToSpawn;
	protected float accumulator;

	public static AudioSource backgroundMusic;
	public static bool isAudioOn;
	public AudioClip coinSFX;
	public AudioClip bombSFX;
	public AudioClip popSFX;
	public AudioClip clickSFX;
	AudioClip sfx;
	protected AudioSource audioSource;

	private int currentSum = 0;

	protected bool isShowedTutorial = false;//was the tutorial page showed?
	//this is abstract class, it shouldn't have Start() or Update()

	Vector3 originalCameraPosition = new Vector3(0, 0.15f, -10f);
	public float shakeAmount = 0;

	/// <summary>
	/// Sets the sum.
	/// </summary>
	/// <param name="s">Sum</param>
	public static void SetSum(int s){
		sum = s;
	}

	protected void Initialization(){
		backgroundMusic = Camera.main.GetComponent<AudioSource> ();//background music audio source is attached to the main camera
		if (PlayerPrefs.GetInt ("isMusicOn") == 0) {//0 = true = music is on, 1 = false = music is off
			backgroundMusic.mute = false;
		} else {
			backgroundMusic.mute = true;
		}
		isAudioOn = PlayerPrefs.GetInt ("isAudioOn") == 0;//0 = true = audio is on, 1 = false = audio is off
		audioSource = GetComponent<AudioSource> ();
		isPaused = false;
		isInputDisable = false;
		accumulator = timeToSpawn;
		score = 0;
		index = 0;
		selectedNodes = new HashSet<Node> ();
		isWatchedAds = false;
		if (PlayerPrefs.GetInt ("NextSum") == 0)
			SetSum (UnityEngine.Random.Range (10, 19));
		else {
			SetSum (PlayerPrefs.GetInt ("NextSum"));
			PlayerPrefs.SetInt ("NextSum", 0);
		}
		UIManager.UpdateSumText (sum);
	}

	protected void GetInput<T>() where T : Node{
		RaycastHit hit;
		if (Input.touchCount == 1 && !isInputDisable) {
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
								if(node.value == 0){
									if(node.myPowerUp == PowerUp.coin)
										sfx = coinSFX;
									else if(node.myPowerUp == PowerUp.bomb)
										sfx = bombSFX;
								}
								node.HideQuad();
							}
						}
						catch(NullReferenceException e){
							Debug.Log (e.Message);
							selectedNodes.Clear ();
						}

						if (currentSum == sum) {
							if (sfx.Equals (bombSFX)) {
								InvokeRepeating("CameraShake", 0, .01f);
								Invoke("StopShaking", 0.3f);
							}
							if(isAudioOn)
								audioSource.PlayOneShot (sfx);
							score += selectedNodes.Count;
							if (score < 72) {
								timeToSpawn = -0.015f * score + 2f;//using an equation to model this y = -0.015x + 2(y is timeToSpawn and x is score)
							}
							else if (score <= 125 && score >= 100 && GameObject.FindGameObjectsWithTag ("Node").Length <= 20) {
								StartCoroutine ("juice1", 2);
							}
							else if (score <= 150 && score >= 125 && GameObject.FindGameObjectsWithTag ("Node").Length <= 20) {
								StartCoroutine ("juice1", 3);
							}
							else if (score >= 150 && GameObject.FindGameObjectsWithTag ("Node").Length <= 20) {
								StartCoroutine ("juice1", 4);
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

	IEnumerator juice1(int num){
		isPaused = true;
		yield return new WaitForSeconds (0.75f);
		for (int a = 0; a < num; a++) {
			for(int i = 0; i < 6; i++){
				Instantiate (bricks [UnityEngine.Random.Range (0, 6)], spawnPoints [i].position, Quaternion.Euler (0, 180, 0));
			}
			yield return new WaitForSeconds (2.0f);
		}
		isPaused = false;
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

	protected void CameraShake()
	{
		if(shakeAmount > 0) 
		{
			Vector3 temp = UnityEngine.Random.insideUnitSphere * shakeAmount;
			temp.z = -10f;
			Camera.main.transform.position = temp;
		}
	}

	protected void StopShaking()
	{
		CancelInvoke("CameraShake");
		Camera.main.transform.position = originalCameraPosition;
	}
}
