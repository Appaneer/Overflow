using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class LevelManager : MonoBehaviour {

	public GameObject[] bricks;
	public static int score;
	public Transform[] spawnPoints;
	public int totalNode;//total # of bricks/node inside the game
	protected int index;
	public static int sum;
	public HashSet<Node> selectedNodes;
	public Vector3 lastNodeSelected;//the transform of the last node user had selected
	public static bool isPaused = false;
	public static bool isInputDisable;
	public GameObject bomb;
	public GameObject coin;
	public static bool isWatchedAds;
	public float timeToSpawn;
	protected float accumulator;
	protected bool isJuicing;
	private int currentSum = 0;
	public static int levelNumber;//0 = langing page 1 = space 2 = stack
	public static LevelManager instance;

	public static AudioSource backgroundMusic;
	public static bool isAudioOn;
	public AudioClip coinSFX;
	public AudioClip bombSFX;
	public AudioClip popSFX;
	public AudioClip clickSFX;
	AudioClip sfx;
	protected AudioSource audioSource;

	protected bool isShowedTutorial = false;//was the tutorial page showed?

	Vector3 originalCameraPosition = new Vector3(0, 0.15f, -9f);
	public float shakeAmount = 0; 

	public static void SetSum(int s){
		sum = s;
	}

	protected void Initialization(){
		instance = this;
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
		isJuicing = false;
		accumulator = timeToSpawn;
		if (levelNumber == 1)
			timeToSpawn = 0.65f;
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
						if(lastNodeSelected != Vector3.zero && Vector3.Distance(tempNode.transform.position, lastNodeSelected) >= 1.4f){
							tempNode.HideQuad();
							foreach (T node in selectedNodes) {
								node.HideQuad ();
							}
							selectedNodes = new HashSet<Node> ();
							UIManager.DisableSumText ();
							currentSum = 0;
						}
						else if(selectedNodes.Add (tempNode) && isAudioOn)
							audioSource.PlayOneShot (clickSFX);
						lastNodeSelected = tempNode.transform.position;
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
						lastNodeSelected = Vector3.zero;
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
							UIManager.updateScore (score);
							totalNode -= selectedNodes.Count;
							if (timeToSpawn > 0.9f && levelNumber == 2) {
								timeToSpawn = -0.006f * score + 2f;//using an equation to model this y = -0.015x + 2(y is timeToSpawn and x is score)
							}
							if (!isJuicing) {
								totalNode = GameObject.FindGameObjectsWithTag ("Node").Length;
								if(totalNode <= 10){
									StartCoroutine ("juice1", 2);
								}
								else if(score <= 100 && score >= 50 && totalNode <= 15){
									StartCoroutine ("juice1", 2);
								}
								else if (score <= 125 && score >= 100 && totalNode <= 20) {
									StartCoroutine ("juice1", 2);
								}
								else if (score <= 150 && score >= 125 && totalNode <= 25) {
									StartCoroutine ("juice1", 2);
								}
								else if (score >= 150 && totalNode <= 30) {
									StartCoroutine ("juice1", 2);
								}
							}
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
		else if(Input.touchCount == 2){
			foreach (T node in selectedNodes) {
				node.HideQuad ();
			}
			selectedNodes = new HashSet<Node> ();
			currentSum = 0;
			UIManager.DisableSumText ();
		}
	}

	IEnumerator juice1(int num){
		isJuicing = true;
		isPaused = true;
		yield return new WaitForSeconds (1.25f);
		for (int a = 0; a < num; a++) {
			for(int i = 0; i < 6; i++){
				Instantiate (bricks [UnityEngine.Random.Range (0, 6)], spawnPoints [i].position, Quaternion.Euler (0, 180, 0));
			}
			yield return new WaitForSeconds (2.0f);
		}
		isJuicing = false;
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
				totalNode++;
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
			temp.z = -9f;
			Camera.main.transform.position = temp;
		}
	}

	protected void StopShaking()
	{
		CancelInvoke("CameraShake");
		Camera.main.transform.position = originalCameraPosition;
	}
}
