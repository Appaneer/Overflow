using UnityEngine;
using System.Collections;

public class RanColors : MonoBehaviour {

	Renderer rend;
	float acc;
	float timeToChange=1f;
	void Start () {
		rend = GetComponent<Renderer> ();
		acc = timeToChange;
		Destroy (gameObject, 12);
	}
	
	// Update is called once per frame
	void Update () {
		acc -= Time.deltaTime;
		if(acc<0)
		{
		rend.material.color = new Color (Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), Random.Range (0.0f, 1.0f), 1);
			acc = timeToChange;
		}
	}
}
