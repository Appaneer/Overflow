using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class TutorialText : MonoBehaviour {

	public Text text;
	public bool fade=false;
	void Start () {
		text = GetComponent<Text> ();
		StartCoroutine(wait (2f));
	}

	IEnumerator wait(float seconds)
	{
		yield return new WaitForSeconds (seconds);
		fade = true;
	}
	void Update()
	{
		if (fade) {
			Color startColor = text.color;
			text.color = Color.Lerp (startColor, Color.clear, Time.deltaTime * 6);
		}
	}
}
