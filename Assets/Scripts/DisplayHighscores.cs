using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayHighscores : MonoBehaviour {

	public Text[] highschoreText;
	Highscores highscoreManager;

	void Start(){
		for(int i = 0; i < highschoreText.Length; i++){
			highschoreText[i].text = i+1 + "Fetching....";
		}

		highscoreManager = GetComponent<Highscores> ();

		StartCoroutine ("RefreshHighScores");
	}

	public void OnHighscoresDownloaded(Highscore[] highScoresList){
		for(int i = 0; i < highschoreText.Length; i++){
			highschoreText[i].text = i+1 + ".";
			if(highScoresList.Length > i){
				highschoreText [i].text += highScoresList [i].username + "- " + highScoresList [i].score;s
			}
		}
	}

	IEnumerator RefreshHighScores(){
		while(true){
			highscoreManager.DownloadHighScore ();
			yield return new WaitForSeconds (30);
		}
	}
}
