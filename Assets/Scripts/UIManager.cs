﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.Net;
using System;

public class UIManager : MonoBehaviour {
	//------langing scene-------
	public Canvas gamePage;//this canvas contains everything but shop page
	public Canvas creditPage;
	public Canvas settingPage;
	public Canvas shopPage;
	public Canvas purchasedPage;
	public Button shopButton;
	public Button creditButton;
	public Button settingButton;
	public Button shareButton;
	public Sprite soundOnSprite;
	public Sprite soundOffSprite;
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
	private bool flag;//if true then reward coins after ads, if false then delete nodes(chance to continue game) after ads
	public Animator gameOverAnim;
	public Animator displaySumAnim;
	public Text targetSumText;
	public Text displaySumText;
	private const string FACEBOOK_URL = "http://www.facebook.com/dialog/feed";
	private const string FACEBOOK_APP_ID = "794667970397816";
	public string gameId;
	public bool enableTestMode;
	private static UIManager instance;

	void Start(){
		instance = this;
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
		
	public void LoadLandingPage(){
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


}
