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
	public Canvas creditPage;
	public Canvas settingPage;
	public Canvas shopPage;
	public Canvas purchasedPage;
	public Button shopButton;
	public Button creditButton;
	public Button settingButton;
	public Button shareButton;
	public Button soundButton;
	public Sprite soundOnSprite;
	public Sprite soundOffSprite;
	public GameObject Border;
	public Text randomText;
	public Text coinText;
	public Animator playButtonAnim;
	//-------tetris/space scene------
	public Canvas endGameCanvas;
	public Button videoButton;
	public Text scoreText;
	public Text scoreText2;
	public Text highScoreText;
	public Text coinText2;
	public Text coinEarnedInGameText;
	private bool flag;//if true then reward coins after ads, if false then delete nodes(chance to continue game) after ads
	public Animator gameOverAnim;
	public Animator coinEarnedAnim;
	public Text targetSumText;
	public Text displaySumText;
	private const string FACEBOOK_URL = "http://www.facebook.com/dialog/feed";
	private const string FACEBOOK_APP_ID = "1182354041784013";
	public string gameId;
	public bool enableTestMode;
	public static UIManager instance;
	public Canvas pauseCanvas;
	public Canvas tutorialCanvas;

	void Start(){
		for(int number = 1; number <= 6; number++){
			UIManager.updateText (GameObject.Find(number+" text").GetComponent<Text>(), PlayerPrefs.GetInt("Num"+number));
		}
		UIManager.updateText (GameObject.Find("freeze").GetComponent<Text>(), PlayerPrefs.GetInt("Freeze"));
		if (PlayerPrefs.GetInt ("isAudioOn") == 0)
			soundButton.image.sprite = soundOnSprite;
		else
			soundButton.image.sprite = soundOffSprite;
		instance = this;
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
		
	void Update(){
		if (Input.GetKey (KeyCode.Escape))
			LoadLandingPage ();
	}

	public void Play(){
		playButtonAnim.SetTrigger ("Play");
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

	public void ToggleTutorialPage(){
		StartCoroutine ("Wait");
	}

	IEnumerator Wait(){
		tutorialCanvas.enabled = true;
		yield return new WaitForSeconds (2.5f);
		tutorialCanvas.enabled = false;
	}

	public void ToggleCreditPage(){
		ToggleButton ();
		creditPage.enabled =  creditPage.enabled ? false : true;
		settingButton.enabled = settingButton.enabled ? false : true;
		shareButton.enabled = shareButton.enabled ? false : true;
	}

	public void ToggleSettingPage(){
		ToggleButton ();
		settingPage.enabled = settingPage.enabled ? false : true;
		creditButton.enabled = creditButton.enabled ? false : true;
		shareButton.enabled = shareButton.enabled ? false : true;
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
			PlayerPrefs.SetInt ("isAudioOn", 1);//1 = audio is on
			LevelManager.isAudioOn = false;
		}
		else{
			PlayerPrefs.SetInt ("isAudioOn", 0);//0 = audio is off
			LevelManager.isAudioOn = true;
		}
	}

	public static void Pause(){
		LevelManager.isPaused = true;
	}

	public void TogglePause(){
		LevelManager.isPaused = !LevelManager.isPaused;
		Time.timeScale = Time.timeScale == 1 ? 0 : 1; 
		Camera.main.farClipPlane = Camera.main.farClipPlane == 1000f ? 10f : 1000f; 
		pauseCanvas.enabled = !pauseCanvas.enabled;
	}
		
	public void TogglePurchasedPage(){
		purchasedPage.enabled = !purchasedPage.enabled;
	}

	public void UseFreezePowerup(){
		if (PlayerPrefs.GetInt ("Freeze") > 0) {
			StartCoroutine ("Freeze");
			PlayerPrefs.SetInt ("Freeze", PlayerPrefs.GetInt("Freeze") - 1);
			UIManager.updateText (GameObject.Find("freeze").GetComponent<Text>(), PlayerPrefs.GetInt("Freeze"));
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
			CoinManager.Withdraw (price);
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
			CoinManager.Withdraw (price);
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
			CoinManager.Withdraw (price);
			updateCoin ();
		}
	}

	public static void ShowEndGamePage(){
		Pause ();
		if (LevelManager.isWatchedAds)
			instance.ShowGameOverPage ();
		else {
			if(instance.endGameCanvas.enabled == false)
				CoinManager.Deposit (LevelManager.score / 10);
			instance.endGameCanvas.enabled = true;
		}
			
	}

	public void ShowGameOverPage(){
		if(LevelManager.score > PlayerPrefs.GetInt("HighScore")){
			PlayerPrefs.SetInt ("HighScore", LevelManager.score);
		}
		gameOverAnim.SetTrigger ("GameOver");
		scoreText2.text = "Score\n"+LevelManager.score;
		highScoreText.text = "High Score\n"+PlayerPrefs.GetInt ("HighScore");
		coinText2.text = ""+PlayerPrefs.GetInt ("Coins");
		coinEarnedInGameText.text = "+" + (LevelManager.score / 10);

		PlayerPrefs.SetInt ("NextSum", 0);
		coinEarnedAnim.SetTrigger ("GameOver");
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
		LevelManager.DeleteNodes (4);
		yield return new WaitForSeconds (0.75f);
		LevelManager.isPaused = false;
		LevelManager.isWatchedAds = true;
		instance.endGameCanvas.enabled = false;
	}

	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			if (flag) {
				CoinManager.Deposit (20);
				updateCoin ();
				coinEarnedAnim.SetTrigger ("GameOver");
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

	public void FacebookShare(){
		ShareToFacebook ("http://www.google.com/", "name", "caption", "description", "http://lmsotfy.com/so.png", "http://www.facebook.com/");
	}

	/// <summary>
	/// Shares to facebook.
	/// </summary>
	/// <param name="linkParameter">Link.</param>
	/// <param name="nameParameter">Name.</param>
	/// <param name="captionParameter">Caption.</param>
	/// <param name="descriptionParameter">Description.</param>
	/// <param name="pictureParameter">Picture.</param>
	/// <param name="redirectParameter">Redirect.</param>
	void ShareToFacebook (string linkParameter, string nameParameter, string captionParameter, string descriptionParameter, string pictureParameter, string redirectParameter)
	{
		Application.OpenURL (FACEBOOK_URL + "?app_id=" + FACEBOOK_APP_ID +
			"&link=" + WWW.EscapeURL(linkParameter) +
			"&name=" + WWW.EscapeURL(nameParameter) +
			"&caption=" + WWW.EscapeURL(captionParameter) + 
			"&description=" + WWW.EscapeURL(descriptionParameter) + 
			"&picture=" + WWW.EscapeURL(pictureParameter) + 
			"&redirect_uri=" + WWW.EscapeURL(redirectParameter));
	}
}
