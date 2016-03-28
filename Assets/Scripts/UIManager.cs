using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.Net;

public class UIManager : MonoBehaviour {

	public Canvas creditPage;
	public Canvas settingPage;
	public Canvas numberPage;
	static Canvas gameOverCanvas;
	public Button numberButton;
	public Sprite soundOnSprite;
	public Sprite soundOffSprite;
	static Text scoreText;
	private const string FACEBOOK_URL = "http://www.facebook.com/dialog/feed";
	private const string FACEBOOK_APP_ID = "794667970397816";
	public string gameId;
	public bool enableTestMode;

	void Start(){
		gameOverCanvas = GameObject.Find ("Game Over Canvas").GetComponent<Canvas>();
		scoreText = GameObject.Find("score text").GetComponent<Text>();

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
			Application.Quit ();
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

	public static void ShowEndGamePage(){
		TogglePause ();
		gameOverCanvas.enabled = true;
	}

	public void ToggleCreditPage(){
		ToggleButton ();
		creditPage.enabled =  creditPage.enabled ? false : true;
	}

	public void ToggleSettingPage(){
		ToggleButton ();
		settingPage.enabled = settingPage.enabled ? false : true;
	}

	public void ToggleNumberPage(){
		ToggleButton ();
		numberPage.enabled = numberPage.enabled ? false : true;
	}

	private void ToggleButton(){
		numberButton.image.enabled = numberButton.enabled ? false : true;
		numberButton.enabled = numberButton.enabled ? false : true;
	}

	public void ToggleSound(Button soundButton){
		soundButton.image.sprite = soundButton.image.sprite.name.Equals ("sound on") ? soundOffSprite : soundOnSprite;
	}

	public static void TogglePause(){
		LevelManager.isPaused = true;
	}

	public void ShowRewardedAd()
	{
		if (Advertisement.IsReady("rewardedVideo"))
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
			LevelManager.DeleteNodes (15);
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

	public static void updateScore(int score){
		scoreText.text = score+"";
	}
		
	public static bool isHavingWiFi()
	{
		try
		{
			using (WebClient client = new WebClient())
				using (var stream = client.OpenRead("http://www.google.com"))
				{
					return true;
				}
		}
		catch
		{
			return false;
		}
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
