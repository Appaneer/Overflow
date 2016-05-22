using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.Net;
using System;

public class UIManager : MonoBehaviour {
	//------langing scene-------
	public Canvas gamePage;//this canvas contains everything but shop page
	public Text highScore;
	public GameObject Border;
	public Text randomText;
	public Text coinText;
	//-------tetris/space scene------
	public Canvas endGameCanvas;
	public Button videoButton;
	public Text scoreText;
	public Text scoreText2;
	public Text highScoreText;
	public Text coinText2;
	public Animator gameOverAnim;
	public Animator displaySumAnim;
	public Text targetSumText;
	public Text displaySumText;
	private static UIManager instance;

	void Start(){
		instance = this;
		if (highScore != null)
			highScore.text = "HIGH SCORE:\n" + PlayerPrefs.GetInt ("HighScore");
	}

	public static void TriggerDisplaySumText(int sum){
		instance.displaySumText.text = "" + sum;
		instance.StartCoroutine ("blahblahblah");
	}

	IEnumerator blahblahblah(){
		instance.displaySumAnim.SetBool ("DisplaySum",true);
		yield return new WaitForSeconds (0.3f);
		instance.displaySumAnim.SetBool ("DisplaySum",false);
	}

	public void LoadTetrisLevel(){
		SceneManager.LoadScene ("Tetris Level");
	}

	public void LoadSpaceLevel(){
		SceneManager.LoadScene ("Space Level");
	}
		
	public static void LoadLandingPage(){
		SceneManager.LoadScene ("Landing Page");
	}

	IEnumerator DeleteNodes(){
		LevelManager.DeleteNodes (8);
		yield return new WaitForSeconds (0.75f);
		LevelManager.isPaused = false;
		instance.endGameCanvas.enabled = false;
	}

	public static void updateText(Text txt, int value){
		txt.text = value + "";
	}
		
	public static void updateScore(int score){
		instance.scoreText.text = score+"";
	}

	public static void updateCoin(){
		instance.coinText.text = PlayerPrefs.GetInt ("Coins")+"";
	}
		
	public static void UpdateSumText(int sum){
		instance.targetSumText.text = "SUM: " + sum;
	}

	public static void TogglePause(){
		LevelManager.isPaused = true;
	}

	public static void ShowEndGamePage(){
		TogglePause ();
		instance.endGameCanvas.enabled = true;
	}

	public static void UpdateCurrentSum(int sum){
		instance.displaySumText.enabled = true;
		instance.displaySumText.text = "" + sum;
	}

	public static void DisableSumText(){
		instance.displaySumText.enabled = false;
	}
}
