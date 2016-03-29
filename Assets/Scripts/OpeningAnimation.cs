using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class OpeningAnimation : MonoBehaviour {

	void Start() {
		StartCoroutine(load (6f));
	}
	IEnumerator load(float time)
	{
		yield return new WaitForSeconds (time);
		SceneManager.LoadScene ("Landing Page");
	}
}
