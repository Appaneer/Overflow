using UnityEngine;
using System.Collections;

public class Highscores : MonoBehaviour {

	const string privateCode = "JPQFg14VEEiS_ZOJdcX8HQ3LjeQdszs02H721h_wE8TQ";
	const string publicCode = "56f88a876e51b6045863c739";
	const string webURL = "http://dreamlo.com/lb/";

	public Highscore[] highScoresList;
	static Highscores instance;
	DisplayHighscores highscoresDisplay;

	void Awake(){
		instance = this;
		highscoresDisplay = GetComponent<DisplayHighscores> ();
	}

	public static void AddNewHighScore(string username, int score){
		instance.StartCoroutine (instance.UploadNewHighScore(username,score));
	}

	public void DownloadHighScore(){
		StartCoroutine ("DownloadHighScoreFromDatabase");
	}

	IEnumerator UploadNewHighScore(string username, int score){
		WWW www = new WWW (webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			Debug.Log ("Upload High Score Successful");
			DownloadHighScore ();
		}
		else
			Debug.LogError ("Error Uploading high score: "+ www.error);

	}

	IEnumerator DownloadHighScoreFromDatabase(){
		WWW www = new WWW (webURL + publicCode + "/pipe/");
		yield return www;
		if (string.IsNullOrEmpty (www.error)) {
			FormatHighScores (www.text);
			highscoresDisplay.OnHighscoresDownloaded (highScoresList);
		}
		else
			Debug.LogError ("Error downloading: "+ www.error);

	}

	void FormatHighScores(string textStream){
		string[] entries = textStream.Split (new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
		highScoresList = new Highscore[entries.Length];
		for(int i = 0; i < entries.Length; i++){
			string[] entryInfo = entries [i].Split (new char[] {'|'});
			highScoresList [i] = new Highscore (entryInfo [0], int.Parse (entryInfo [1]));
		}
	}
}

public struct Highscore{

	public string username;
	public int score;

	public Highscore(string _username, int _score){
		username = _username;
		score = _score;
	}
}