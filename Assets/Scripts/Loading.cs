using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Loading : MonoBehaviour {

	public Text text;
	public GameObject logo;
	public float interval = 0.025f;
	void Start () {
		text = GetComponent<Text> ();
		StartCoroutine(execute ());

	}

	IEnumerator execute()
	{
		text.text = "";
		string phrase="------ TETRA CODERS PRESENTS ------";
		for (int i = 0; i < phrase.Length; i++) {
			add (phrase [i]+"");
			yield return new WaitForSeconds (interval);
		}
	}

	void add(string letter)
	{
		text.text = text.text + letter;
	}

	void fastPrint(string phrase)
	{
		for (int i = 0; i < phrase.Length; i++) {
			add (phrase [i]+"");
		}
	}

}
