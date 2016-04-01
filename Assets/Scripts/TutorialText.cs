using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class TutorialText : MonoBehaviour {

	void Start () {
		StartCoroutine(wait (3.0f));
	}

	IEnumerator wait(float seconds)
	{
		GetComponent<Text>().enabled = true;
		yield return new WaitForSeconds (seconds);
		GetComponent<Text> ().enabled = false;
	}
}
