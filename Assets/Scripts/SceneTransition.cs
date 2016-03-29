
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour {
	public Image FadeImg;
	float fadeSpeed = 5f;
	bool fade=false;
	void Start() {
		FadeImg.rectTransform.localScale = new Vector2 (Screen.width, Screen.height);
		StartCoroutine (load (4f));
	}

	void  Update() {
		if(fade)
		FadeToBlack ();

	}
	IEnumerator load(float time)
	{
		yield return new WaitForSeconds (time);
		fade = true;
	}

	void FadeToBlack()
	{
		FadeImg.color = Color.Lerp(FadeImg.color, Color.black, fadeSpeed * Time.deltaTime);
	}

}
