using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.Net;
using System;
public class UIManager : MonoBehaviour {

	public Canvas gamePage;//this canvas contains everything but shop page
	public Canvas creditPage;
	public Canvas settingPage;
	static Canvas gameOverCanvas;
	public Canvas shopPage;
	public Canvas purchasedPage;
	public Button shopButton;
	public Sprite soundOnSprite;
	public Sprite soundOffSprite;
	public Text randomText;
	static Text coinText;
	static Text scoreText;
	private const string FACEBOOK_URL = "http://www.facebook.com/dialog/feed";
	private const string FACEBOOK_APP_ID = "794667970397816";
	public string gameId;
	public bool enableTestMode;

	void Start(){
		gameOverCanvas = GameObject.Find ("Game Over Canvas").GetComponent<Canvas>();
		scoreText = GameObject.Find("score text").GetComponent<Text>();
		coinText = GameObject.Find("coin text").GetComponent<Text>();

		updateCoin ();
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

	public void LoadTetrisLevel(){
		SceneManager.LoadScene ("Tetris Level");
	}

	public void LoadSpaceLevel(){
		SceneManager.LoadScene ("Space Level");
	}

	public void LoadLandingPage(){
		SceneManager.LoadScene ("Landing Page");
	}

	public void ToggleCreditPage(){
		ToggleButton ();
		creditPage.enabled =  creditPage.enabled ? false : true;
	}

	public void ToggleSettingPage(){
		ToggleButton ();
		settingPage.enabled = settingPage.enabled ? false : true;
	}

	public void ToggleShopPage(){
		gamePage.enabled = gamePage.enabled ? false : true;
		shopPage.enabled = shopPage.enabled ? false : true;
	}

	private void ToggleButton(){
		shopButton.image.enabled = shopButton.enabled ? false : true;
		shopButton.enabled = shopButton.enabled ? false : true;
	}

	public void ToggleSound(Button soundButton){
		soundButton.image.sprite = soundButton.image.sprite.name.Equals ("sound on") ? soundOffSprite : soundOnSprite;
	}

	public static void TogglePause(){
		LevelManager.isPaused = true;
	}

	public void BuyBomb(int price){
		if(PlayerPrefs.GetInt("Coins") >= price){
			//buy stuff
			PlayerPrefs.SetInt("NumBomb", PlayerPrefs.GetInt("NumBomb") + 1);
			CoinManager.Withdraw (price);
			updateCoin ();
			StartCoroutine ("DisplayPurchasedPage");
		}
	}

	public void BuyHorizontal(int price){
		if(PlayerPrefs.GetInt("Coins") >= price){
			//buy stuff
			PlayerPrefs.SetInt("NumHor", PlayerPrefs.GetInt("NumHor") + 1);
			CoinManager.Withdraw (price);
			updateCoin ();
			StartCoroutine ("DisplayPurchasedPage");
		}
	}

	public void BuyVertical(int price){
		if(PlayerPrefs.GetInt("Coins") >= price){
			//buy stuff
			PlayerPrefs.SetInt("NumVer", PlayerPrefs.GetInt("NumVer") + 1);
			CoinManager.Withdraw (price);
			updateCoin ();
			StartCoroutine ("DisplayPurchasedPage");
		}
	}
		
	public void BuyRandomNumber(GameObject thisButton){
		int price = 1;
		if(PlayerPrefs.GetInt("Coins") >= price){
			CoinManager.Withdraw (price);
			updateCoin ();
			thisButton.SetActive (false);//disable this gameobject
			randomText.enabled = true;
			StartCoroutine (RandomNumberAnimation(thisButton));
			StartCoroutine ("DisplayPurchasedPage");
		}
	}

	private IEnumerator RandomNumberAnimation(GameObject thisButton){
		for(int i = 30; i >= 1; i--){
			randomText.text = UnityEngine.Random.Range (11,20)+"";
			yield return new WaitForSeconds (0.4f / i);
		}
		yield return new WaitForSeconds (3);
		Debug.Log ("Your number is "+int.Parse(randomText.text));
		thisButton.SetActive (true);
		randomText.enabled = false;
	}

	public static void ShowEndGamePage(){
		TogglePause ();
		gameOverCanvas.enabled = true;
	}

	public void ShowRewardedAd()
	{
		if (Advertisement.IsReady("rewardedVideo") && isHavingWiFi())
		{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("rewardedVideo", options);
		}
	}

	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			LevelManager.DeleteNodes (10);
			LevelManager.isPaused = false;
			gameOverCanvas.enabled = false;
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

	public static void updateScore(int score){
		scoreText.text = score+"";
	}
		
	public static void updateCoin(){
		coinText.text = PlayerPrefs.GetInt ("Coins")+"";
	}

	IEnumerator DisplayPurchasedPage(){
		purchasedPage.enabled = true;
		yield return new WaitForSeconds (1);
		purchasedPage.enabled = false;
	}

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
		
	}

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
