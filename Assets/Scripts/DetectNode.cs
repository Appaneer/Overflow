using UnityEngine;
using System.Collections;

public class DetectNode : MonoBehaviour {
	
	float timeToSpawn = 1f;
	protected float accumulator;
	public GameObject redBar;
	void Start () {
		accumulator = timeToSpawn;
	}

	void onTriggerEnter(){
		redBar.GetComponent<MeshRenderer> ().enabled = false;
		accumulator -= Time.deltaTime;
		if (accumulator <= 0.0f) {
			accumulator = timeToSpawn;
		}
	}
}
