using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class TextColors : MonoBehaviour {

	Text rend;
	float acc;
	float timeToChange=1f;
	void Start () {
		rend = GetComponent<Text> ();
		acc = timeToChange;
	}
	
	// Update is called once per frame
	void Update () {
		acc -= Time.deltaTime;
		if(acc<0)
		{
		rend.color = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), 1);
			acc = timeToChange;
		}
	}
}
