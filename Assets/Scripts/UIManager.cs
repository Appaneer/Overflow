	using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Net;
using System;

public class UIManager : MonoBehaviour {
	//------langing scene-------
	public Canvas gamePage;//this canvas contains everything but shop page
	public Canvas creditPage;
	public Canvas settingPage;
	public Canvas shopPage;
	public Canvas purchasedPage;
	public Canvas exitPage;
	public Button shopButton;
	public Button creditButton;
	public Button settingButton;
	public Button shareButton;
	public Button musicButton;
	public Sprite musicOnSprite;
	public Sprite musicOffSprite;
	public Button soundButton;
	public Sprite soundOnSprite;
	public Sprite soundOffSprite;
	public Text coinText;
	public Animator playButtonAnim;
	public ParticleSystem coinParticleEffect;
	//-------tetris/space scene------
	public Canvas endGameCanvas;
	public Button videoButton;
	public Text scoreText;
	public Text scoreText2;
	public Text highScoreText;
	public Text coinEarnedInGameText;
	private bool flag;//if true then reward coins after ads, if false then delete nodes(chance to continue game) after ads
	private static bool isGameEnded;
	public Animator gameOverAnim;
	public Text targetSumText;
	public Text displaySumText;
	public string gameId;
	public bool enableTestMode;
	public static UIManager instance;
	public Canvas pauseCanvas;
	public Canvas tutorialCanvas;

	public AudioClip freezeSFX;
	public AudioMixerSnapshot paused;
	public AudioMixerSnapshot unpaused;

	void Start(){
		isGameEnded = false;
		for(int number = 1; number <= 6; number++){
			UIManager.updateText (GameObject.Find(number+" text").GetComponent<Text>(), PlayerPrefs.GetInt("Num"+number));
		}
		UIManager.updateText (GameObject.Find("freeze").GetComponent<Text>(), PlayerPrefs.GetInt("Freeze"));
		instance = this;
		if (LevelManager.levelNumber != 3) {//if this is not tutorial
			unpaused.TransitionTo (0.01f);
			if (PlayerPrefs.GetInt ("isAudioOn") == 0)
				soundButton.image.sprite = soundOnSprite;
			else
				soundButton.image.sprite = soundOffSprite;
			if (PlayerPrefs.GetInt ("isMusicOn") == 0)
				musicButton.image.sprite = musicOnSprite;
			else
				musicButton.image.sprite = musicOffSprite;
			if (string.IsNullOrEmpty(gameId)) { // Make sure the Game ID is set.
				Debug.LogError("Failed to initialize Unity Ads. Game ID is null or empty.");
			} else if (!Advertisement.isSupported) {
				Debug.LogWarning("Unable to initialize Unity Ads. Platform not supported.");
			} else if (Advertisement.isInitialized) {
				Debug.Log("Unity Ads is already initialized.");
			} else {
				Debug.Log(string.Format("Initialize Unity Ads using Game ID {0} with Test Mode {1}.",
					gameId, enableTestMode ? "enabled" : "disabled"));
				Advertisement.Initialize(gameId, enableTestMode);
			}
		}
	}
		
	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if(LevelManager.levelNumber == 0)
				exitPage.enabled = true;
			else
				ReturnHome ();
		}
	}

	public void DisableExitPage(){
		exitPage.enabled = false;
	}

	public void ExitGame(){
		Application.Quit ();
	}

	public void Play(){
		playButtonAnim.SetTrigger ("Play");
	}

	public void LoadTetrisLevel(){
		LevelManager.levelNumber = 2;
		StartCoroutine ("LoadWithWait", "Tetris Level");
	}

	public void LoadSpaceLevel(){
		LevelManager.levelNumber = 1;
		StartCoroutine ("LoadWithWait", "Space Level");
	}

	public void LoadLandingPage(){
		pauseCanvas.enabled = false;
		LevelManager.levelNumber = 0;
		StartCoroutine ("LoadWithWait", "Landing Page");
	}

	IEnumerator LoadWithWait(string sceneName){
		yield return new WaitForSeconds (2f);
		SceneManager.LoadScene (sceneName);
	}

	public void ToggleTutorialPage(){
		tutorialCanvas.enabled = !tutorialCanvas.enabled;
	}

	public void ToggleCreditPage(){
		if (!settingPage.enabled) {
			creditPage.enabled = !creditPage.enabled;
			gamePage.enabled = !gamePage.enabled;
		}
	}

	public bool isOnSettingPage = false;

	public void ToggleSettingPage(){
		isOnSettingPage = !isOnSettingPage;
		if (isOnSettingPage == false) {
			settingPage.enabled = false;
			creditPage.enabled = false;
			shopButton.enabled = true;
			shopButton.image.enabled = true;
		}
		else if(isOnSettingPage == true) {
			settingPage.enabled = true;
			creditPage.enabled = false;
			shopButton.enabled = false;
			shopButton.image.enabled = false;
		}
	}

	public void ToggleShopPage(){
		gamePage.enabled = gamePage.enabled ? false : true;
		shopPage.enabled = shopPage.enabled ? false : true;

		updateCoin ();
	}

	private void ToggleButton(){
		shopButton.image.enabled = shopButton.enabled ? false : true;
		shopButton.enabled = shopButton.enabled ? false : true;
	}

	public void ToggleSound(Button soundButton){
		soundButton.image.sprite = soundButton.image.sprite.name.Equals ("sound on") ? soundOffSprite : soundOnSprite;
		if (PlayerPrefs.GetInt ("isAudioOn") == 0) {
			PlayerPrefs.SetInt ("isAudioOn", 1);
			LevelManager.isAudioOn = false;
		}
		else{
			PlayerPrefs.SetInt ("isAudioOn", 0);
			LevelManager.isAudioOn = true;
		}
	}

	public void ToggleMusic(Button musicButton){
		musicButton.image.sprite = musicButton.image.sprite.name.Equals ("music-on") ? musicOffSprite : musicOnSprite;
		if (PlayerPrefs.GetInt ("isMusicOn") == 0) {
			PlayerPrefs.SetInt ("isMusicOn", 1);
			try{
				LevelManager.backgroundMusic.mute = true;
			}
			catch(Exception e){
				//do nothing!!!!
			}
		}
		else{
			PlayerPrefs.SetInt ("isMusicOn", 0);
			try{
				LevelManager.backgroundMusic.mute = false;
			}
			catch(Exception e){
				//do nothing!!!!
			}
		}
	}

	public static void Pause(){
		LevelManager.isPaused = true;
		LevelManager.isInputDisable = true;
	}

	public void TogglePause(){
		if (!isGameEnded) {
			LevelManager.isPaused = !LevelManager.isPaused;
			LevelManager.isInputDisable = !LevelManager.isInputDisable;
			Time.timeScale = Time.timeScale == 1 ? 0 : 1; 
			Camera.main.farClipPlane = Camera.main.farClipPlane == 1000f ? 10f : 1000f; 
			pauseCanvas.enabled = !pauseCanvas.enabled;
			if (Time.timeScale == 0)
				paused.TransitionTo (0.01f);
			else
				unpaused.TransitionTo (0.01f);
		}
	}
		
	public void ReturnHome(){
		LevelManager.isPaused = !LevelManager.isPaused;
		LevelManager.isInputDisable = !LevelManager.isInputDisable;
		Time.timeScale = 1;
		pauseCanvas.enabled = false;
		LevelManager.backgroundMusic.volume = 0.0f;
		LoadLandingPage ();
	}

	public void TogglePurchasedPage(){
		purchasedPage.enabled = !purchasedPage.enabled;
	}

	public void UseFreezePowerup(){
		if (PlayerPrefs.GetInt ("Freeze") > 0) {
			StartCoroutine ("Freeze");
			PlayerPrefs.SetInt ("Freeze", PlayerPrefs.GetInt("Freeze") - 1);
			UIManager.updateText (GameObject.Find("freeze").GetComponent<Text>(), PlayerPrefs.GetInt("Freeze"));
			GetComponent<AudioSource> ().PlayOneShot (freezeSFX);
		}
	}

	IEnumerator Freeze(){
		Time.timeScale = 0.2f;
		yield return new WaitForSeconds (2f);
		Time.timeScale = 1;
	}

	public void BuyFreezePowerup(){
		int price = 20;
		if(PlayerPrefs.GetInt("Coins") >= price){
			//buy stuff
			TogglePurchasedPage();
			PlayerPrefs.SetInt("Freeze", PlayerPrefs.GetInt("Freeze") + 1);
			PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") - price);
			updateCoin ();
		}
		UIManager.updateText (GameObject.Find("freeze").GetComponent<Text>(), PlayerPrefs.GetInt("Freeze"));
	}

	public void BuyNumberPowerups(int number){
		int price = 20;
		if(PlayerPrefs.GetInt("Coins") >= price && number >= 1 && number <= 6){
			//buy stuff
			TogglePurchasedPage();
			PlayerPrefs.SetInt("Num"+number, PlayerPrefs.GetInt("Num"+number) + 1);
			PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") - price);
			updateCoin ();
		}
		UIManager.updateText (GameObject.Find(number+" text").GetComponent<Text>(), PlayerPrefs.GetInt("Num"+number));
	}

	public void BuySum(int number){
		int price = 0;
		switch(number){
		case 10:
			price = 40;
			break;
		case 11:
			price = 35;
			break;
		case 12:
			price = 30;
			break;
		}
		if (PlayerPrefs.GetInt ("Coins") >= price) {
			TogglePurchasedPage();
			PlayerPrefs.SetInt ("NextSum", number);
			PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") - price);
			updateCoin ();
		}
	}

	public static void ShowEndGamePage(){
		Pause ();
		isGameEnded = true;
		if (LevelManager.isWatchedAds)
			instance.ShowGameOverPage ();
		else {
			if(instance.endGameCanvas.enabled == false)
				PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") + LevelManager.score / 10);
			instance.endGameCanvas.enabled = true;
		}
	}

	public void ShowGameOverPage(){
		Node[] bigbodies = GameObject.FindObjectsOfType<Node> ();
		foreach(Node r in bigbodies){
			r.Destroy ();	
		}
		if (LevelManager.levelNumber == 1) {//space mode
			if (LevelManager.score > PlayerPrefs.GetInt ("HighScoreSpace"))
				PlayerPrefs.SetInt ("HighScoreSpace", LevelManager.score);
			highScoreText.text = "High Score\n"+PlayerPrefs.GetInt ("HighScoreSpace");
		} else {
			if(LevelManager.score > PlayerPrefs.GetInt("HighScoreStack"))
				PlayerPrefs.SetInt ("HighScoreStack", LevelManager.score);
			highScoreText.text = "High Score\n"+PlayerPrefs.GetInt ("HighScoreStack");
		}
		StartCoroutine (TextAnimation(scoreText2, LevelManager.score, "SCORE\n", 0.002f));
		gameOverAnim.SetTrigger ("GameOver");
		StartCoroutine (TextAnimation(coinEarnedInGameText, LevelManager.score / 10, "COINS EARNED\n+", 0.05f));
		coinText.text = ""+PlayerPrefs.GetInt ("Coins");
		PlayerPrefs.SetInt ("NextSum", 0);
	}

	IEnumerator TextAnimation(Text txt, int number, string message, float speed){
		yield return new WaitForSeconds (1f);
		for(int i = 0; i <= number; i++){
			txt.text = message + i;
			yield return new WaitForSeconds (speed);
		}
	}

	public void ShowRewardedAd(bool isCoinRewarded)
	{
		flag = isCoinRewarded;
		if (Advertisement.IsReady("rewardedVideo") && isHavingWiFi())
		{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("rewardedVideo", options);
		}
	}
		
	IEnumerator DeleteNodes(){
		if (LevelManager.levelNumber == 1) {
			LevelManager.DeleteNodes (GameObject.FindGameObjectsWithTag ("Node").Length - 55);
			LevelManager.instance.totalNode = 55;
		}
		else
			LevelManager.DeleteNodes (6);
		
		yield return new WaitForSeconds (0.75f);
		LevelManager.isPaused = false;
		LevelManager.isInputDisable = false;
		LevelManager.isWatchedAds = true;
		instance.endGameCanvas.enabled = false;
		isGameEnded = false;
	}

	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			if (flag) {
				PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins") + 20);
				updateCoin ();
				coinParticleEffect.Emit (20);
			} else 
				StartCoroutine ("DeleteNodes");
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}

	public static void updateText(Text txt, int value){
		txt.text = value + "";
	}

	/// <summary>
	/// Updates the current score.
	/// </summary>
	/// <param name="score">Score.</param>
	public static void updateScore(int score){
		instance.scoreText.text = score+"";
	}

	/// <summary>
	/// Updates amount of coin that the player has.
	/// </summary>
	public static void updateCoin(){
		instance.coinText.text = PlayerPrefs.GetInt ("Coins")+"";
	}

	/// <summary>
	/// Updates the sum.
	/// </summary>
	/// <param name="sum">Sum.</param>
	public static void UpdateSumText(int sum){
		instance.targetSumText.text = "SUM: " + sum;
	}

	public static void UpdateCurrentSum(int sum){
		instance.displaySumText.enabled = true;
		instance.displaySumText.text = "" + sum;
	}

	public static void DisableSumText(){
		instance.displaySumText.enabled = false;
	}

	/// <summary>
	/// Is the device connected to internet
	/// </summary>
	/// <returns><c>true</c>, device has internet connection, <c>false</c> device doesn't have internet connection.</returns>
	public static bool isHavingWiFi()
	{
		#if UNITY_EDITOR
		if (Network.player.ipAddress.ToString() != "127.0.0.1")
			return true;   
		return false;
		#endif
		#if UNITY_IPHONE || UNITY_ANDROID
		if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
			return true;
		return false;
		#endif
	}

	public void RateApp(){
		#if UNITY_ANDROID
		Application.OpenURL("market://details?id=YOUR_ID");
		#elif UNITY_IPHONE
		Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
		#endif
	}

	public void LikeFacebook(){
		Application.OpenURL("https://wwww.facebook.com/111percent");
	}
}
